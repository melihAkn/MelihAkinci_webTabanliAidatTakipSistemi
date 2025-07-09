import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

const LoginPage = () => {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [role, setRole] = useState(3)
  const navigate = useNavigate()
    const dumyUsername = "melihaeeekinci"
    const dummyPassword = "1234sad5678"
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
        credentials: 'include', // HttpOnly cookie için şart
        body: JSON.stringify({ 
            "username" : dumyUsername,
            "password" : dummyPassword })
      })

      if (!res.ok) {
        const errorText = await res.json()
        alert(errorText.detail)
        return
      }

      // Giriş başarılı → Kullanıcı bilgilerini al

      const getUserInfosEndpoint =
      role === "apartmentManager"
        ? 'http://localhost:5263/manager/GetUserRole'
        : 'http://localhost:5263/resident/GetUserRole'

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
        value={dumyUsername}
        onChange={(e) => setUsername(e.target.value)}
      />
      <br />

      <input
        placeholder="Şifre"
        type="password"
        value={dummyPassword}
        onChange={(e) => setPassword(e.target.value)}
      />
      <br />

      <button onClick={handleLogin}>Giriş Yap</button>
    </div>
  )
}

export default LoginPage