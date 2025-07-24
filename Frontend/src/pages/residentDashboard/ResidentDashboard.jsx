import { useState } from 'react'
import './residentDashboard.css'
import Card from '../../components/card'
import UpdateResidentInfoCard from '../../components/resident/UpdateResidentInfoCard'
import MyPaymentNotificationsCard from '../../components/resident/MyPaymentNotifications'
import MyMaintenanceFeesCard from '../../components/resident/myMaintenanceFeeCard'
import MySpecialFeesCard from '../../components/resident/MySpecialFeeCard'
import MyApartmentAndUnit from '../../components/resident/MyApartmentAndUnit'
const ResidentDashboard = () => {
  const [cards, setCards] = useState([])


  const logout = async () => {
    try {
      const res = await fetch('http://localhost:5263/auth/resident/logout', {
        method: 'GET',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        }
      })
      if (!res.ok) {
        alert('çıkış yapma başarısız')
        return
      } else {
        window.location.reload()
      }
    } catch (err) {
      console.error('Silme hatası:', err)
    }
  }

  const updateInfos = async () => {
    try {
      setCards([<UpdateResidentInfoCard key="update-Manager-info" />])
    } catch (err) {
      console.error(err)
    }
  }

  const mymaintenanceFees = async () => {
    try {
      setCards([<MyMaintenanceFeesCard key="my-Maintenance-fee" onAction={createPaymentNotification} />])
    } catch (err) {
      console.error(err)
    }
  }

  const mySpecialFees = async () => {
    try {
      setCards([<MySpecialFeesCard key="my-Special-fee" onAction={createPaymentNotification} />])
    } catch (err) {
      console.error(err)
    }
  }

  const apartmentAndUnitInfos = async () => {
    try {
      setCards([<MyApartmentAndUnit key="my-Apartment-Unit"  />])
    } catch (err) {
      console.error(err)
    }
  }

  const myPaymentNotifications = async () => {
    try {
      setCards([<MyPaymentNotificationsCard key="my-payment-notifications" />])
    } catch (err) {
      console.error(err)
    }
  }

  const createPaymentNotification = async () => {
    try {
      const res = await fetch("http://localhost:5263/resident/create-payment-notification", {
        method: "POST",
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

  return (
    <div id="appContainer">
      <div id="sideStyleNavBar">
        <ul>
          <li onClick={() => updateInfos()}>bilgilerim</li>
          <li onClick={() => mymaintenanceFees()}>aidat ücretlerim</li>
          <li onClick={() => mySpecialFees()}>özel ücretlerim</li>
          <li onClick={() => apartmentAndUnitInfos()}>apartman ve daire bilgilerim</li>
          <li onClick={() => myPaymentNotifications()}>ödeme bildirimlerim</li>




          <li onClick={() => logout()}>çıkış yap</li>

        </ul>
      </div>

      <div id="contentPanel">
        {cards.map((card, index) =>
          typeof card === 'object' && !card.type ? (
            <Card key={index} data={card} onDefineSpecialFee={handleDefineSpecialFee} />
          ) : (
            <div key={index}>{card} </div>
          )
        )}
      </div>
    </div>
  )
}

export default ResidentDashboard