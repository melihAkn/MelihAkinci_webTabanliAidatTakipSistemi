import { useEffect, useState } from "react"

const ApartmentCard = ({ onAction }) => {
  const [apartments, setApartments] = useState([])

  useEffect(() => {
    const getApartments = async () => {
      try {
        const res = await fetch("http://localhost:5263/manager/getApartments", {
          method: "GET",
          credentials: "include",
        })

        if (!res.ok) {
          setApartments([{ error: "Hata oluştu." }])
          return
        }

        const data = await res.json()
        setApartments(Array.isArray(data) ? data : [data])
      } catch (err) {
        console.error(err)
        setApartments([{ error: "Sunucu hatası" }])
      }
    }

    getApartments()
  }, [])

  return (
    <>
      {apartments.map((apartment) => (
        <div
          key={apartment.apartmentId}
          style={{
            border: "1px solid #ccc",
            borderRadius: "8px",
            padding: "1rem",
            marginBottom: "1rem",
            backgroundColor: "#f9f9f9",
          }}
        >
          <h4>{apartment.name}</h4>
          <p><strong>Adres:</strong> {apartment.address}</p>
          <p><strong>Maksimum Kat Maliki:</strong> {apartment.maxAmountOfResidents}</p>
          <p><strong>Aidat Tutarı:</strong> {apartment.maintenanceFeeAmount} ₺</p>
          <p><strong>Kat Sayısı:</strong> {apartment.floorCount}</p>
          <p><strong>Her Katta Daire:</strong> {apartment.apartmentUnitCountForEachFloor}</p>

          <div style={{ marginTop: "1rem" }}>
            <button onClick={() => onAction?.("AddApartmentUnit", apartment.apartmentId)}>
              apartmana daire ekle
            </button>
            <button onClick={() => onAction?.("viewResidents", apartment.apartmentId)}>
              Kat Maliklerini Görüntüle
            </button>

            <button onClick={() => onAction?.("unPaidMaintenanceFees", apartment.apartmentId)}>
              ödenmemiş aidatlar
            </button>

            <button onClick={() => onAction?.("unPaidSpecialFees", apartment.apartmentId)}>
              ödenmemiş özel ücretler
            </button>
            <button onClick={() => onAction?.("paidMaintenanceFees", apartment.apartmentId)}>
              ödenmiş olup ödeme onayı bekleyen aidatlar
            </button>
            <button onClick={() => onAction?.("paidSpecialFees", apartment.apartmentId)}>
              ödenmiş olup ödeme onayı bekleyen özel ücretler
            </button>

            <button onClick={() => onAction?.("setMaintenanceFeeToAllResidents", apartment.apartmentId)}>
              apartman da ki kat maliklerine otomatik aidat tanımla
            </button>

            <button
              onClick={() => onAction?.("delete", apartment.apartmentId)}
              style={{ color: "red" }}
            >
              Sil
            </button>
          </div>
        </div>
      ))}
    </>
  )
}

export default ApartmentCard
