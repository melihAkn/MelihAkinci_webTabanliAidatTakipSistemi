import { useEffect, useState } from "react"
const MyPaymentNotifications = ({ onAction }) => {
  const [paymentNotifications, setPaymentNotifications] = useState([])
  useEffect(() => {
    const getPaymentNotifications = async () => {
      try {
        const res = await fetch("http://localhost:5263/manager/get-payment-notifications", {
          method: "POST",
          headers: {
            'Content-Type': 'application/json'
          },
          credentials: "include",
          body: JSON.stringify({
            "apartmentId": 0
          })
        })
        if (!res.ok) {
          setPaymentNotifications([{ error: "Hata oluştu." }])
          return
        }

        const data = await res.json()
        setPaymentNotifications(Array.isArray(data) ? data : [data])
        console.log(data)
      } catch (err) {
        console.error(err)
        setPaymentNotifications([{ error: "Sunucu hatası" }])
      }
    }

    getPaymentNotifications()
  }, [])

  return (
    <>
      {paymentNotifications.map((paymentNotification) => (
        <div
          key={paymentNotification.notificationId}
          style={{
            border: "1px solid #ccc",
            borderRadius: "8px",
            padding: "1rem",
            marginBottom: "1rem",
            backgroundColor: "#f9f9f9",
          }}
        >
          <h4>{paymentNotification.paymentName}</h4>
          <p><strong>ödeme bilgileri</strong></p>
          <p>tutar: {paymentNotification.amount}</p>
          <p>ödeme açıklaması: {paymentNotification.paymentDescription}</p>
          <p>ödeme tarihi: {paymentNotification.paymentDate}</p>
          <p>son ödeme tarihi: {paymentNotification.dueDate}</p>
          <p>ödeme durumu: {paymentNotification.status}</p>
          <p>dekont resmi(henuz dahil edilmedi): {paymentNotification.receiptUrl}</p>

          <p><strong>kat maliki bilgileri</strong></p>
          <p>kat maliki mesajı: {paymentNotification.notificationMessage}</p>
          <p>kat maliki adı: {paymentNotification.residentName}</p>
          <p>kat maliki soyadı: {paymentNotification.residentSurname}</p>
          <p>kat maliki mail adresi: {paymentNotification.residentEmail}</p>
          <p>kat maliki telefon numarası: {paymentNotification.residentPhoneNumber}</p>

          <p><strong>Apartman bilgileri</strong></p>
          <p>apartman adı: {paymentNotification.apartmentName}</p>
          <p>apartman adresi: {paymentNotification.apartmentAddress}</p>

          <div style={{ marginTop: "1rem" }}>

            <button onClick={() => onAction?.("allowPaymentNotification", 0, paymentNotification.notificationId)}>
              Onayla
            </button>
            <br />
            <form
              onSubmit={(e) => {
                e.preventDefault(); // sayfanın yeniden yüklenmesini engelle
                const denyMessage = e.target.elements[0].value; // veya: e.target["0"].value
                onAction?.("denyPaymentNotification", 0, paymentNotification.notificationId, denyMessage);
              }}
            >
              <input type="text" required></input>
              <button>
                Reddet
              </button>

            </form>
          </div>
        </div>
      ))}
    </>
  )
}

export default MyPaymentNotifications
