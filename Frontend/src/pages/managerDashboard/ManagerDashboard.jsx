import { useState } from 'react'
import './ManagerDashboard.css'
import Card from '../../components/card'
import AddApartmentCard from '../../components/addApartmentCard'
import UpdateManagerInfoCard from '../../components/UpdateManagerInfoCard'
import ApartmentCard from '../../components/apartmentCard'
import GetApartmentUnit from '../../components/getApartmentUnitsCard'
const ManagerDashboard = () => {
  const [cards, setCards] = useState([])
  const updateInfos = async (endpoint) => {
    try {
      setCards([<UpdateManagerInfoCard key="update-Manager-info" />])
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


  const getApartments = () => {
    setCards([
      <ApartmentCard key="get-apartments" onAction={handleApartmentAction} />
    ])
  }

  const handleDefineSpecialFee = async (cardData) => {
    // Burada, 'Özel Ücret Tanımla' butonuna tıklandığında yapılacakları yazabilirsin.
    // Örneğin:
    console.log('Özel ücret tanımlama isteği geldi:', cardData)
    // Burada modal açabilir, form gösterebilir veya başka state güncelleyebilirsin.
    try {
      const res = await fetch('http://localhost:5263/manager/setSpecificFeeToApartmentResident', {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        // borç adı name, açıklama description, tutar amount, id
        body: JSON.stringify({
          apartmentResidentId: cardData.apartmentResidents.id



        }) // DTO'nun property adı burada
      })
      if (!res.ok) {
        alert('Silme işlemi başarısız oldu.')
        return
      }
      alert('Apartman silindi.')
      getApartments()
    } catch (err) {
      console.error('Silme hatası:', err)
      alert('Sunucu hatası')
    }



  }
  const handleApartmentAction = async (action, apartmentId) => {
    switch (action) {
      case 'delete':
        if (window.confirm('Apartmanı silmek istediğinize emin misiniz?')) {
          try {
            const res = await fetch('http://localhost:5263/manager/deleteApartment', {
              method: 'DELETE',
              credentials: 'include',
              headers: {
                'Content-Type': 'application/json'
              },
              body: JSON.stringify({ apartmentId }) // DTO'nun property adı burada
            })
            if (!res.ok) {
              alert('Silme işlemi başarısız oldu.')
              return
            }
            alert('Apartman silindi.')
            getApartments()
          } catch (err) {
            console.error('Silme hatası:', err)
            alert('Sunucu hatası')
          }
        }
        break
      case 'AddApartmentUnit':
        console.log("asd")
        break

      case 'unPaidMaintenanceFees':
        try {
          const res = await fetch('http://localhost:5263/manager/getUnPaidMaintenanceFees', {
            method: 'POST',
            credentials: 'include',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ apartmentId })
          })
          const data = await res.json()
          setCards(Array.isArray(data) ? data : [data])
        } catch (err) {
          console.error(err)
          alert('Aidatlar alınamadı')
        }
        break

      case 'unPaidSpecialFees':
        try {
          const res = await fetch('http://localhost:5263/manager/getUnPaidSpecialFees', {
            method: 'POST',
            credentials: 'include',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ apartmentId })
          })
          const data = await res.json()
          setCards(Array.isArray(data) ? data : [data])
        } catch (err) {
          console.error(err)
          alert('Aidatlar alınamadı')
        }
        break
      case 'paidMaintenanceFees':
        try {
          const res = await fetch('http://localhost:5263/manager/getAllPaidMaintenanceFees', {
            method: 'POST',
            credentials: 'include',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ apartmentId })
          })
          const data = await res.json()
          setCards(Array.isArray(data) ? data : [data])
        } catch (err) {
          console.error(err)
          alert('Aidatlar alınamadı')
        }
        break

      case 'paidSpecialFees':
        try {
          const res = await fetch('http://localhost:5263/manager/getAllPaidSpecialFees', {
            method: 'POST',
            credentials: 'include',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ apartmentId })
          })
          const data = await res.json()
          setCards(Array.isArray(data) ? data : [data])
        } catch (err) {
          console.error(err)
          alert('Aidatlar alınamadı')
        }
        break


      case 'viewResidents':
        try {
          const res = await fetch('http://localhost:5263/manager/getApartmentsUnits', {
            method: 'POST',
            credentials: 'include',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ apartmentId })
          })
          const data = await res.json()
          const cards = data.map((unit, idx) => (
            <GetApartmentUnit
              key={idx}
              data={unit}
              onDefineSpecialFee={handleDefineSpecialFee}
            />
          ))
          setCards(cards)


          //setCards([<GetApartmentUnit data={data} />])
        } catch (err) {
          console.error(err)
          alert('Kat malikleri alınamadı')
        }
        break
      case 'setMaintenanceFeeToAllResidents':
        try {
          const res = await fetch('http://localhost:5263/manager/setMaintenanceFeeToAllResidents', {
            method: 'POST',
            credentials: 'include',
            headers: {
              'Content-Type': 'application/json'
            },
            body: JSON.stringify({ apartmentId })
          })
          const data = await res.json()
          if(!res.ok){
            alert(data.detail)
          }
            alert(data.message)


        } catch (err) {
          console.error(err)
          alert('Kat malikleri alınamadı')
        }
        break

        
      default:
        console.warn('Bilinmeyen işlem:', action)
    }

  }


  return (
    <div id="appContainer">
      <div id="sideStyleNavBar">
        <ul>
          <li onClick={() => updateInfos()}>bilgilerim</li>
          <li onClick={() => getApartments('manager/getApartments')}>apartmanlarım</li>
          <li onClick={() => addApartment()}>apartman ekle</li>


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

export default ManagerDashboard