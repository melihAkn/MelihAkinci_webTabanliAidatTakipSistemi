import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

const LoginPage = () => {
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [role, setRole] = useState(3) // default olarak kat maliki
  const navigate = useNavigate()
    const dumyUsername = "melihaeeekinci"
    const dummyPassword = "1234sad5678"
  const handleLogin = async () => {
    const endpoint =
  role === 2
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
      role === 2
        ? 'http://localhost:5263/manager/GetUserRoleId'
        : 'http://localhost:5263/resident/GetUserRoleId'

      const infoRes = await fetch(getUserInfosEndpoint, {
        method: 'GET',
        credentials: 'include'
      })

      if (!infoRes.ok) {
        alert('Kullanıcı bilgisi alınamadı!')
        return
      }
      // backend den role id dönüyor 
      // 1 admin
      // 2 apartmen manager -- apartman yöneticisi
      // 3 apartment resident -- kat maliki
      const userInfo = await infoRes.json()
      const userRoleId = userInfo.roleId
      if (userRoleId === 2) navigate('/manager')
      else if (userRoleId === 3) navigate('/owner')
      else if(userRoleId === 1) navigate('/admin')
      else alert('Bilinmeyen rol!')

    } catch (err) {
      console.error('Giriş hatası:', err)
      alert('Sunucu hatası')
    }
  }

  return (
    <div>
      <h2>Giriş</h2>

      <select value={role} onChange={(e) => setRole(Number(e.target.value))}>
        <option value="3">Kat Maliki</option>
        <option value="2">Yönetici</option>
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