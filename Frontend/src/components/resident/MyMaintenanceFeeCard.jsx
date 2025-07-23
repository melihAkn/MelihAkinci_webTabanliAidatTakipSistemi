import { useEffect, useState } from "react"
import CreatePaymentNotificationForm from "./CreatePaymentNotificationForm"

const MyMaintenanceFeesCard = ({ onAction }) => {
    const [maintenanceFees, setMaintenanceFees] = useState([])
    const [formVisibleMap, setFormVisibleMap] = useState({}) // her fee için form görünürlüğü
    const getMyMaintenanceFees = async () => {
            try {
                const res = await fetch("http://localhost:5263/resident/my-apartment-unit-maintenance-fees", {
                    method: "GET",
                    credentials: "include",
                })

                if (!res.ok) {
                    setMaintenanceFees([{ error: "Hata oluştu." }])
                    return
                }

                const data = await res.json()
                setMaintenanceFees(Array.isArray(data) ? data : [data])
            } catch (err) {
                console.error(err)
                setMaintenanceFees([{ error: "Sunucu hatası" }])
            }
        }
    useEffect(() => {
    

        getMyMaintenanceFees()
    }, [])

    const toggleForm = (feeId) => {
        setFormVisibleMap((prev) => ({
            ...prev,
            [feeId]: !prev[feeId]
        }))
    }

    const handleSubmitFee = async (feeData) => {
        try {
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
            getMyMaintenanceFees()

            // formu kapat
            if (feeData.maintenanceFeeId) {
                setFormVisibleMap((prev) => ({
                    ...prev,
                    [feeData.maintenanceFeeId]: false
                }))
            }
        } catch (err) {
            console.error(err)
        }
    }

    return (
        <>
            {maintenanceFees.map((fee) => (
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
                    <h4>aidat</h4>
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
                                        maintenanceFeeId={fee.id}
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
