import { logout } from '../../auth'
import { useNavigate } from 'react-router-dom'

const OwnerDashboard = () => {
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div>
      <h2>Kat Maliki Paneli</h2>
      <button onClick={handleLogout}>Çıkış</button>
    </div>
  )
}

export default OwnerDashboard