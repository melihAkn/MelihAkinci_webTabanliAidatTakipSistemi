import { Navigate } from 'react-router-dom'
import { getRole } from '../auth'

const ProtectedRoute = ({ children, allowedRole }) => {
  const role = getRole()
  if (!role) return <Navigate to="/login" />
  if (role !== allowedRole) return <Navigate to="/login" />
  return children
}

export default ProtectedRoute