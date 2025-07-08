import { logout } from '../auth'
import { useNavigate } from 'react-router-dom'

const ManagerDashboard = () => {
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div>
      <h2>Yönetici Paneli</h2>
      <button onClick={handleLogout}>Çıkış</button>
    </div>
  )
}

export default ManagerDashboard