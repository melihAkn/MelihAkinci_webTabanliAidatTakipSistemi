import { useState, useEffect } from 'react'

const UpdateResidentInfoCard = () => {
  const [name, setName] = useState('')
  const [surname, setSurname] = useState('')
  const [phoneNumber, setPhoneNumber] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [newPasswordAgain, setNewPasswordAgain] = useState('')

  useEffect(() => {
    // İlk yüklemede kullanıcı bilgilerini çek
    const fetchUserInfo = async () => {
      try {
        const res = await fetch('http://localhost:5263/resident/get-user-infos', {
          method: 'GET',
          credentials: 'include'
        })

        if (!res.ok) return

        const data = await res.json()
        setName(data.name)
        setSurname(data.surname)
        setPhoneNumber(data.phoneNumber)
        setEmail(data.email)
      } catch (err) {
        console.error('Bilgi alınamadı', err)
      }
    }

    fetchUserInfo()
  }, [])

  const handleSubmit = async (e) => {
    e.preventDefault()

    if (password.length == 0) {
      alert("şifre boş olamaz")
    }
    let updatedInfo
    if (newPassword == newPasswordAgain && newPassword.length > 0 && newPasswordAgain.length > 0) {

      updatedInfo = {
        name,
        surname,
        phoneNumber,
        email,
        password,
        newPassword
      }
    } else if (newPassword.length == 0 && newPasswordAgain.length == 0){
      updatedInfo = {
        name,
        surname,
        phoneNumber,
        email,
        password
      }
    }

    try {
      const res = await fetch('http://localhost:5263/resident/update-resident-infos', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify(updatedInfo)
      })

      if (!res.ok) {
        const err = await res.json()
        alert(`Hata: ${err.detail || 'Güncelleme başarısız'}`)
        return
      }
      const response = await res.json()
      console.log(response.message)
      alert(response.message)
    } catch (err) {
      console.error(err)
      alert('Sunucu hatası')
    }

}

return (
  <div>
    <h3>Bilgilerimi Güncelle</h3>
    <form onSubmit={handleSubmit}>
      <label>Ad:</label>
      <input type="text" value={name} onChange={e => setName(e.target.value)} /><br />

      <label>Soyad:</label>
      <input type="text" value={surname} onChange={e => setSurname(e.target.value)} /><br />

      <label>Telefon:</label>
      <input type="text" value={phoneNumber} onChange={e => setPhoneNumber(e.target.value)} /><br />

      <label>Email:</label>
      <input type="email" value={email} onChange={e => setEmail(e.target.value)} /><br />

      <label>Şifre:</label>
      <input type="password" onChange={e => setPassword(e.target.value)} /><br />

      <label>guncellemek istemiyorsanız boş bırakın</label><br />
      <label>Yeni Şifre:</label>
      <input type="password" onChange={e => setNewPassword(e.target.value)} /><br />

      <label>Yeni Şifre tekrar:</label>
      <input type="password" onChange={e => setNewPasswordAgain(e.target.value)} /><br />

      <button type="submit">Güncelle</button>
    </form>
  </div>
)
}

export default UpdateResidentInfoCard
