import { useState } from 'react'

const AddApartmentCard = () => {
  const [name, setName] = useState('')
  const [maxResidents, setMaxResidents] = useState('')
  const [address, setAddress] = useState('')
  const [fee, setFee] = useState('')
  const [floorCount, setFloorCount] = useState('')
  const [unitCountPerFloor, setUnitCountPerFloor] = useState('')

  const handleSubmit = async (e) => {
    e.preventDefault()

    const newApartment = {
      name,
      maxAmountOfResidents: parseInt(maxResidents),
      address,
      maintenanceFeeAmount: parseFloat(fee),
      floorCount: parseInt(floorCount),
      apartmentUnitCountForEachFloor: parseInt(unitCountPerFloor),
    }

    try {
      const res = await fetch('http://localhost:5263/manager/addApartment', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify(newApartment)
      })

      if (!res.ok) {
        const err = await res.json()
        alert(`Hata: ${err.detail || 'Sunucu hatası'}`)
        return
      }

      alert('Apartman başarıyla eklendi.')

    } catch (err) {
      console.error(err)
      alert('Sunucu hatası')
    }
  }

  return (
    <div>
      <h3>Yeni Apartman Ekle</h3>
      <form id="addNewApartment" onSubmit={handleSubmit}>
        <label>Apartman adı</label>
        <input type="text" value={name} onChange={e => setName(e.target.value)} placeholder="Apartman Adı" /><br />

        <label>Apartman da bulunabilecek en fazla kat maliki sayısı</label>
        <input type="number" value={maxResidents} onChange={e => setMaxResidents(e.target.value)} placeholder="Kat Maliki Sayısı"/><br />

        <label>Apartman adresi</label>
        <input type="text" value={address} onChange={e => setAddress(e.target.value)} placeholder="Apartman Adresi" /><br />

        <label>Apartman Aidatı</label>
        <input type="number" value={fee} onChange={e => setFee(e.target.value)} placeholder="Aidat Tutarı"/><br />

        <label>Kat sayısı</label>
        <input type="number" value={floorCount} onChange={e => setFloorCount(e.target.value)} placeholder="Kat Sayısı" /><br />

        <label>Her katta bulunan daire sayısı</label>
        <input type="number" value={unitCountPerFloor} onChange={e => setUnitCountPerFloor(e.target.value)} placeholder="Daire Sayısı" /><br />

        <button type="submit">Apartman Ekle</button>
      </form>
    </div>
  )
}

export default AddApartmentCard
