import { useEffect, useState } from "react"
import CreatePaymentNotificationForm from "./CreatePaymentNotificationForm"
import UpdateMyPaymentNotificationForm from "./UpdateMyPaymentNotificationForm"

const MyPaymentNotificationCard = ({ onAction }) => {
    const [paymentNotifications, setPaymentNotifications] = useState([])
    const [formVisibleMap, setFormVisibleMap] = useState({}) // her özel ücret için form kontrolü
    const getMyPaymentNotifications = async () => {
        try {
            const res = await fetch("http://localhost:5263/resident/get-my-payment-notifications", {
                method: "GET",
                credentials: "include",
            })

            if (!res.ok) {
                const data = await res.json()
                return
            }

            const data = await res.json()
            setPaymentNotifications(data)
            console.log(data)
        } catch (err) {
            console.error(err)
        }
    }
    useEffect(() => {
        getMyPaymentNotifications()
    }, [])

    const toggleForm = (feeId) => {
        setFormVisibleMap((prev) => ({
            ...prev,
            [feeId]: !prev[feeId]
        }))
    }

    const handleSubmitFee = async (feeData) => {
        const formData = new FormData()
        try {

            formData.append("PaymentNotificationId", feeData.paymentNotificationId)
            formData.append("NotificationMessage", feeData.notificationMessage)
            formData.append("ReceiptFile", feeData.file)

            console.log(formData)
            console.log("fee data brada")
            console.log(feeData)
            const res = await fetch("http://localhost:5263/resident/update-payment-notification", {
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
            getMyPaymentNotifications()
            // formu kapat
            if (feeData.specialFeeId) {
                setFormVisibleMap((prev) => ({
                    ...prev,
                    [feeData.specialFeeId]: false
                }))
            }
        } catch (err) {
            console.error(err)
        }
    }

    return (
        <>
            {paymentNotifications.map((pymnt) => (
                <div
                    key={pymnt.id}
                    style={{
                        border: "1px solid #ccc",
                        borderRadius: "8px",
                        padding: "1rem",
                        marginBottom: "1rem",
                        backgroundColor: "#f9f9f9",
                    }}
                >
                    <h4>{pymnt.maintenanceFee == null ? "özel ücret ödemesi" : "aidat ödemesi"}</h4>
                    <p><strong>ödenen ücret:</strong> {pymnt.amount} ₺</p>
                    <p><strong>son ödeme tarihi:</strong> {pymnt.dueDate}</p>
                    <p><strong>ödenme durumu:</strong> {pymnt.status}</p>
                    <p><strong>yöneticiye iletilen mesajınız:</strong> {pymnt.notificationMessage}</p>
                    {pymnt.status == "Reddedildi" && (
                        <p><strong>reddedilme sebebi:</strong> {pymnt.message}</p>
                    )}
                    <p><strong>ödenme tarihi:</strong> {pymnt.paymentDate}</p>
                    <p><strong>dekont url:</strong> {pymnt.receiptUrl}</p>

                    <div style={{ marginTop: "1rem" }}>
                        {pymnt.status == "Beklemede" && (
                            <>
                                <button onClick={() => toggleForm(pymnt.id)}>
                                    {formVisibleMap[pymnt.id] ? "Formu Gizle" : "ödeme bildirimini guncelle"}
                                </button>
                                {formVisibleMap[pymnt.id] && (
                                    <UpdateMyPaymentNotificationForm
                                        key={pymnt.id}
                                        paymentNotificationId={pymnt.id}
                                        onSubmit={handleSubmitFee}
                                        onCancel={() => toggleForm(pymnt.id)}
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

export default MyPaymentNotificationCard
