import { useState } from 'react'
import './ManagerDashboard.css'
import Card from '../../components/card'
import AddApartmentCard from '../../components/addApartmentCard'
import UpdateManagerInfoCard from '../../components/UpdateManagerInfoCard'
const ManagerDashboard = () => {
  const [cards, setCards] = useState([])
  const updateInfos = async (endpoint) => {
    try {
      setCards([<UpdateManagerInfoCard key="add-apartment" />])
    } catch (err) {
      console.error(err)
    }
  }


  const addApartment = async () => {
    try {
      setCards([<AddApartmentCard key="add-apartment" />])
    } catch (err) {
      console.error(err)
    }
  }

  const getApartments = async (endpoint) => {
    try {
      const res = await fetch(`http://localhost:5263/${endpoint}`, {
        method: 'GET',
        credentials: 'include',
      })

      if (!res.ok) {
        setCards([{ error: 'Hata oluştu.' }])
        return
      }

      const data = await res.json()
      console.log(data)
      if (Array.isArray(data)) {
        setCards(data)
      } else {
        setCards([data])
      }
    } catch (err) {
      console.error(err)
      setCards([{ error: 'Sunucu hatası' }])
    }
  }

  const getUnPaidMaintenanceFees = async (endpoint) => {

  }
  const getUnPaidSpecialFees = async (endpoint) => {

  }
  const getAllPaidMaintenanceFees = async (endpoint) => {

  }
  const getAllPaidSpecialFees = async (endpoint) => {

  }


  return (
    <div id="appContainer">
      <div id="sideStyleNavBar">
        <ul>
          <li onClick={() => updateInfos()}>bilgilerim</li>
          <li onClick={() => getApartments('manager/getApartments')}>apartmanlarım</li>
          <li onClick={() => addApartment()}>apartman ekle</li>


          <li onClick={() => handleClick('manager/getUnPaidMaintenanceFees')}>ödenmemiş idatlar</li>
          <li onClick={() => handleClick('manager/getUnPaidSpecialFees')}>ödenmemiş özel ücretler</li>
          <li onClick={() => handleClick('manager/getAllPaidMaintenanceFees')}>ödeme onayı bekleyen aidatlar</li>
          <li onClick={() => handleClick('manager/getAllPaidSpecialFees')}>ödeme onayı bekleyen özel ücretler</li>
        </ul>
      </div>

      <div id="contentPanel">
        {cards.map((card, index) =>
          typeof card === 'object' && !card.type ? (
            // Düz veri objesi → Card component
            <Card key={index} data={card} />
          ) : (
            // JSX bileşeni (örneğin <AddApartmentCard />)
            <div key={index}>{card}</div>
          )
        )}
      </div>
    </div>
  )
}

export default ManagerDashboard