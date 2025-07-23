import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

const LoginPage = () => {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [role, setRole] = useState()
  const navigate = useNavigate()
    const managerUsername = "melihaeeekinci"
    const managerPassword = "deneme123456"
    const ResidentUsername = "mehmet.kut8752"
    const ResidentPassword = "deneme123"
  const handleLogin = async () => {
    const endpoint =
  role === "apartmentManager"
    ? 'http://localhost:5263/auth/manager/login'
    : 'http://localhost:5263/auth/resident/login'

    try {
      const res = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        credentials: "include",
        body: JSON.stringify({ 
            "username" : ResidentUsername,
            "password" : ResidentPassword })
      })

      if (!res.ok) {
        const errorText = await res.json()
        alert(errorText.detail)
        return
      }

      const getUserInfosEndpoint =
      role === "apartmentManager"
        ? 'http://localhost:5263/manager/get-user-role'
        : 'http://localhost:5263/resident/get-user-role'

      const infoRes = await fetch(getUserInfosEndpoint, {
        method: 'GET',
        credentials: 'include'
      })

      if (!infoRes.ok) {
        alert('Kullanıcı bilgisi alınamadı!')
        return
      }
      const userInfo = await infoRes.json()
      const userRoleId = userInfo.userRole
      if (userRoleId === "ApartmentManager") navigate('/manager')
      else if (userRoleId === "ApartmentResident") navigate('/resident')
      else if(userRoleId === "Admin") navigate('/admin')
      else alert('Bilinmeyen rol!')

    } catch (err) {
      console.error('Giriş hatası:', err)
      alert('Sunucu hatası')
    }
  }


  return (
    <div>
      <h2>Giriş</h2>

      <select value={role} onChange={(e) => setRole(e.target.value)}>
        <option value="apartmentResident">Kat Maliki</option>
        <option value="apartmentManager">Yönetici</option>
      </select>
      <br />

      <input
        placeholder="Kullanıcı adı"
        value={username}
        onChange={(e) => setUsername(e.target.value)}
      />
      <br />

      <input
        placeholder="Şifre"
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
      />
      <br />

      <button onClick={handleLogin}>Giriş Yap</button>
    </div>
  )
}

export default LoginPage