import { useState } from "react"
import SpecialFeeForm from "./specialFeeForm"
import AddApartmentResidentForm from "./addApartmentResidentForm"
const GetApartmentUnit = ({ data }) => {
  const [showForm, setShowForm] = useState(false)
  const renderValue = (value) => {
    if (Array.isArray(value)) {
      return (
        <ul>
          {value.map((item, idx) => (
            <li key={idx}>{typeof item === 'object' ? renderObject(item) : item}</li>
          ))}
        </ul>
      )
    } else if (typeof value === 'object' && value !== null) {
      return renderObject(value)
    } else {
      return String(value)
    }
  }

  const renderObject = (obj) => {
    return (
      <div style={{ marginLeft: '1rem', marginBottom: '0.5rem' }}>
        {Object.entries(obj).map(([key, val], idx) => (
          <div key={idx}>
            <strong>{key}:</strong> {renderValue(val)}
          </div>
        ))}
      </div>
    )
  }
  const hasResident =
    data.isHaveResident === true ||
    (Array.isArray(data.apartmentResidents) && data.apartmentResidents.length > 0);

  const residentId = Array.isArray(data.apartmentResidents)
    ? data.apartmentResidents[0]?.id
    : data.apartmentResidents?.id

  const handleAddApartmentResidentSubmit = async (residentData) => {
    
    try {
      const res = await fetch('http://localhost:5263/manager/setApartmentResidentToAnUnit', {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(residentData)
      })
      const response = await res.json()
      console.log(residentData)
      if (!res.ok) {
        console.log(response)
        alert(response.detail)
        return
      }
      alert(response.message)
      setShowForm(false)
      
    } catch (err) {
      console.error(err)
      alert("Sunucu hatası.")
    }
  }

  const handleSubmitFee = async (feeData) => {
    try {
      const res = await fetch('http://localhost:5263/manager/setSpecificFeeToApartmentResident', {
        method: 'POST',
        credentials: 'include',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(feeData)
      })
      if (!res.ok) {
        alert("İşlem başarısız.")
        return
      }

      alert("Özel ücret başarıyla tanımlandı.")
      setShowForm(false)
    } catch (err) {
      console.error(err)
      alert("Sunucu hatası.")
    }
  }

  return (
    <div style={{
      border: '1px solid #ccc',
      borderRadius: '8px',
      padding: '1rem',
      marginBottom: '1rem',
      backgroundColor: '#fafafa',
      boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
    }}>
      {renderObject(data)}
      {hasResident ? (
        <>
          <button
            style={{ marginTop: '1rem' }}
            onClick={() => setShowForm((prev) => !prev)}
          >
            {showForm ? "Formu Gizle" : "Özel Ücret Tanımla"}
          </button>


          <button
            style={{ marginTop: '1rem' }}
            onClick={() => setShowForm((prev) => !prev)}
          >
            {showForm ? "Formu Gizle" : "daire bilgilerini guncelle"}
          </button>

           <button
            style={{ marginTop: '1rem' }}
            onClick={() => setShowForm((prev) => !prev)}
          >
            {showForm ? "Formu Gizle" : "apartman sakinini guncelle"}
          </button>

          {showForm && (
            <SpecialFeeForm
              residentId={residentId}
              onSubmit={handleSubmitFee}
              onCancel={() => setShowForm(false)}
            />
          )}
        </>
      ) : (
        <>
        
          <button
            style={{ marginTop: '1rem' }}
            onClick={() => setShowForm((prev) => !prev)}
          >
            {showForm ? "Formu Gizle" : "Kat maliki ekle"}
          </button>
          <button
            style={{ marginTop: '1rem' }}
            onClick={() => setShowForm((prev) => !prev)}
          >
            {showForm ? "Formu Gizle" : "daire bilgilerini guncelle"}
          </button>

           <button
            style={{ marginTop: '1rem' }}
            onClick={() => setShowForm((prev) => !prev)}
          >
            {showForm ? "Formu Gizle" : "apartman sakinini guncelle"}
          </button>

          {showForm && (
            <AddApartmentResidentForm
              apartmentUnitId={data.apartmentUnitId}
              onSubmit={handleAddApartmentResidentSubmit}
              onCancel={() => setShowForm(false)}
            />
          )}
        </>
      )}
    </div>
  )
}

export default GetApartmentUnit

