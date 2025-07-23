import { useEffect, useState } from "react"
import CreatePaymentNotificationForm from "./CreatePaymentNotificationForm"

const MyMaintenanceFeesCard = ({ onAction }) => {
    const [specialFees, setSpecialFees] = useState([])
    const [formVisibleMap, setFormVisibleMap] = useState({}) // her özel ücret için form kontrolü
    const getMySpecialFees = async () => {
        try {
            const res = await fetch("http://localhost:5263/resident/my-apartment-unit-special-fees", {
                method: "GET",
                credentials: "include",
            })

            if (!res.ok) {
                setSpecialFees([{ error: "Hata oluştu." }])
                return
            }

            const data = await res.json()
            setSpecialFees(Array.isArray(data) ? data : [data])
        } catch (err) {
            console.error(err)
            setSpecialFees([{ error: "Sunucu hatası" }])
        }
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
            const formData = new FormData()
            

            formData.append("MaintenanceFeeId", feeData.maintenanceFeeId)
            formData.append("SpecialFeeId", feeData.specialFeeId)
            formData.append("NotificationMessage", feeData.notificationMessage)
            formData.append("ReceiptFile", feeData.file)
            console.log(formData)
            const res = await fetch("http://localhost:5263/resident/create-payment-notification", {
                method: "POST",
                credentials: "include",
                body: formData
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

export default MyMaintenanceFeesCard
