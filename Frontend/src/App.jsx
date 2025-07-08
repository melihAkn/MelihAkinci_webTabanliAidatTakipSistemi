import { Routes, Route, Navigate } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import ResidentDashboard from './pages/ResidentDashboard'
import ManagerDashboard from './pages/ManagerDashboard'
import ProtectedRoute from './components/ProtectedRoute'
import { getRole } from './auth'

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/owner"
        element={
          <ProtectedRoute allowedRole="owner">
            <ResidentDashboard />
          </ProtectedRoute>
        }
      />
      <Route
        path="/manager"
        element={
          <ProtectedRoute allowedRole="manager">
            <ManagerDashboard />
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<Navigate to="/login" />} />
    </Routes>
  )
}

export default App