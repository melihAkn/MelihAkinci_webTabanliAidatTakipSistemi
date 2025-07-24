import { useEffect, useState } from "react"
import CreatePaymentNotificationForm from "./CreatePaymentNotificationForm"

const MyApartmentAndUnit = () => {
    const [apartmentInfos, setApartmentInfos] = useState([])
    const getMyApartmentAndUnitInfos = async () => {
        try {

            const getApartmentUnitInfos = await fetch('http://localhost:5263/resident/my-apartment-unit', {
                method: 'GET',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
            })

            const getApartmentUnitInfosResponse = await getApartmentUnitInfos.json()
            if (!getApartmentUnitInfos.ok) {
                alert(getApartmentUnitInfosResponse.detail)
                console.log(getApartmentUnitInfosResponse)
                return
            }
            setApartmentInfos(Array.isArray(getApartmentUnitInfosResponse) ? getApartmentUnitInfosResponse : [getApartmentUnitInfosResponse])
        } catch (err) {
            console.error(err)
            alert("Sunucu hatası.")
        }
    }
    useEffect(() => {
        getMyApartmentAndUnitInfos()
    }, [])

    return (
        <>
            {apartmentInfos.map((apartmentUnit) => (
                <div
                    key={apartmentUnit.id}
                    style={{
                        border: "1px solid #ccc",
                        borderRadius: "8px",
                        padding: "1rem",
                        marginBottom: "1rem",
                        backgroundColor: "#f9f9f9",
                    }}
                >
                    <h4>apartman bilgileri</h4>
                    <p>apartman adı: {apartmentUnit.apartmentName}</p>
                    <p>apartman adresi: {apartmentUnit.address}</p>
                    <p>aidat ücreti: {apartmentUnit.maintenanceFeeAmount}</p>

                    <h4>daire bilgileri</h4>
                    <p>bulunduğu kat: {apartmentUnit.floorNumber}</p>
                    <p>apartman numarası: {apartmentUnit.apartmentNumber}</p>
                    <p>apartman dairesi metrekare: {apartmentUnit.squareMeters}</p>
                    <p>apartman dairesi tipi: {apartmentUnit.apartmentType}</p>

                    <h4>ödeme bilgileri</h4>
                    <p>ödemeler için iban: {apartmentUnit.iban}</p>
                    <p>ödemeler için yönetici ad soyadı: {apartmentUnit.managerNameAndSurname}</p>

                </div>
            ))}
        </>
    )
}

export default MyApartmentAndUnit
