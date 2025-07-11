import { useState } from "react"
// forms
import SpecialFeeForm from "./specialFeeForm"
import AddApartmentResidentForm from "./addApartmentResidentForm"
import UpdateApartmentUnitForm from "./updateApartmentUnitForm"
import UpdateResidentForm from "./updateResidentForm"



const GetApartmentUnit = ({ data }) => {
  const [showSpecialFeeForm, setShowSpecialFeeForm] = useState(false)
  const [showUpdateApartmentUnitForm, setShowUpdateApartmentForm] = useState(false)
  const [showUpdateResidentForm, setShowUpdateResidentForm] = useState(false)
  const [showAddResidentForm, setShowAddResidentForm] = useState(false)

  const hasResident =
    data.isHaveResident === true ||
    (Array.isArray(data.apartmentResidents) && data.apartmentResidents.length > 0)

  const residentId = Array.isArray(data.apartmentResidents)
    ? data.apartmentResidents[0]?.id
    : data.apartmentResidents?.id

  const handleAddApartmentResidentSubmit = async (residentData) => {
    try {
      const res = await fetch('http://localhost:5263/manager/setApartmentResidentToAnUnit', {
        method: 'POST',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(residentData)
      })
      const response = await res.json()
      if (!res.ok) {
        alert(response.detail)
        return
      }
      alert(response.message)
      setShowAddResidentForm(false)
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
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(feeData)
      })
      if (!res.ok) {
        alert("İşlem başarısız.")
        return
      }
      alert("Özel ücret başarıyla tanımlandı.")
      setShowSpecialFeeForm(false)
    } catch (err) {
      console.error(err)
      alert("Sunucu hatası.")
    }
  }

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

  const renderObject = (obj) => (
    <div style={{ marginLeft: '1rem', marginBottom: '0.5rem' }}>
      {Object.entries(obj).map(([key, val], idx) => (
        <div key={idx}>
          <strong>{key}:</strong> {renderValue(val)}
        </div>
      ))}
    </div>
  )

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
          <button onClick={() => setShowSpecialFeeForm(prev => !prev)}>
            {showSpecialFeeForm ? "Formu Gizle" : "Özel Ücret Tanımla"}
          </button>
          {showSpecialFeeForm && (
            <SpecialFeeForm
              residentId={residentId}
              onSubmit={handleSubmitFee}
              onCancel={() => setShowSpecialFeeForm(false)}
            />
          )}

          <button onClick={() => setShowUpdateApartmentForm(prev => !prev)}>
            {showUpdateApartmentUnitForm ? "Formu Gizle" : "Daire Bilgilerini Güncelle"}
          </button>
          {showUpdateApartmentUnitForm && (
            <UpdateApartmentUnitForm
              residentId={residentId}
              onSubmit={handleSubmitFee}
              onCancel={() => setShowUpdateApartmentForm(false)}
            />
          )}

          <button onClick={() => setShowUpdateResidentForm(prev => !prev)}>
            {showUpdateResidentForm ? "Formu Gizle" : "Apartman Sakinini Güncelle"}
          </button>
          {showUpdateResidentForm && (
            <UpdateResidentForm
              residentId={residentId}
              onSubmit={handleSubmitFee}
              onCancel={() => setShowUpdateResidentForm(false)}
            />
          )}
        </>
      ) : (
        <>
          <button onClick={() => setShowAddResidentForm(prev => !prev)}>
            {showAddResidentForm ? "Formu Gizle" : "Kat Maliki Ekle"}
          </button>
          {showAddResidentForm && (
            <AddApartmentResidentForm
              apartmentUnitId={data.apartmentUnitId}
              onSubmit={handleAddApartmentResidentSubmit}
              onCancel={() => setShowAddResidentForm(false)}
            />
          )}

          <button onClick={() => setShowUpdateApartmentForm(prev => !prev)}>
            {showUpdateApartmentUnitForm ? "Formu Gizle" : "Daire Bilgilerini Güncelle"}
          </button>
          {showUpdateApartmentUnitForm && (
            <UpdateApartmentUnitForm
              residentId={residentId}
              onSubmit={handleSubmitFee}
              onCancel={() => setShowUpdateApartmentForm(false)}
            />
          )}
          <button onClick={() => setShowUpdateResidentForm(prev => !prev)}>
            {showUpdateResidentForm ? "Formu Gizle" : "Apartman Sakinini Güncelle"}
          </button>
          {showUpdateResidentForm && (
            <UpdateResidentForm
              residentId={residentId}
              onSubmit={handleSubmitFee}
              onCancel={() => setShowUpdateResidentForm(false)}
            />
          )}
        </>
      )}
    </div>
  )
}

export default GetApartmentUnit
