import { useEffect, useState } from "react"
import CreatePaymentNotificationForm from "./CreatePaymentNotificationForm"

const MyApartmentAndUnit = () => {
    const [specialFees, setSpecialFees] = useState([])
    const [formVisibleMap, setFormVisibleMap] = useState({}) // her özel ücret için form kontrolü
        const getMyApartmentAndUnitInfos = async () => {
           
        }
    useEffect(() => {
        getMySpecialFees()
    }, [])

    const toggleForm = (feeId) => {
        setFormVisibleMap((prev) => ({
            ...prev,
            [feeId]: !prev[feeId]
        }))
    }

    const handleSubmitFee = async (feeData) => {
        try {
            console.log(feeData)
            const res = await fetch("http://localhost:5263/resident/create-payment-notification", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify(feeData)
            })

            if (!res.ok) {
                const data = await res.json()
                console.log(data)
                return
            }

            const data = await res.json()
            console.log(data)
            getMySpecialFees()
            // formu kapat
            if (feeData.specialFeeId) {
                setFormVisibleMap((prev) => ({
                    ...prev,
                    [feeData.specialFeeId]: false
                }))
            }
        } catch (err) {
            console.error(err)
            setSpecialFees([{ error: "Sunucu hatası" }])
        }
    }

    return (
        <>
            {specialFees.map((fee) => (
                <div
                    key={fee.id}
                    style={{
                        border: "1px solid #ccc",
                        borderRadius: "8px",
                        padding: "1rem",
                        marginBottom: "1rem",
                        backgroundColor: "#f9f9f9",
                    }}
                >
                    <h4>özel ücret</h4>
                    <p><strong>ödenmesi gereken ücret:</strong> {fee.amount} ₺</p>
                    <p><strong>son ödeme tarihi:</strong> {fee.dueDate}</p>
                    <p><strong>ödenme durumu:</strong> {fee.isPaid ? "ödendi" : "ödenmedi"}</p>
                    <p><strong>ödenme tarihi:</strong> {fee.paymentDate}</p>

                    <div style={{ marginTop: "1rem" }}>
                        {!fee.isPaid && (
                            <>
                                <button onClick={() => toggleForm(fee.id)}>
                                    {formVisibleMap[fee.id] ? "Formu Gizle" : "ödeme yap ve ödeme bildirimi oluştur"}
                                </button>
                                {formVisibleMap[fee.id] && (
                                    <CreatePaymentNotificationForm
                                        key={fee.id}
                                        specialFeeId={fee.id}
                                        onSubmit={handleSubmitFee}
                                        onCancel={() => toggleForm(fee.id)}
                                    />
                                )}
                            </>
                        )}
                    </div>
                </div>
            ))}
        </>
    )
}

export default MyApartmentAndUnit
