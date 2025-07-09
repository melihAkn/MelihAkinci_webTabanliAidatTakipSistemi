import { Routes, Route, Navigate } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import ResidentDashboard from './pages/residentDashboard/ResidentDashboard'
import ManagerDashboard from './pages/managerDashboard/ManagerDashboard'
import ProtectedRoute from './components/ProtectedRoute'

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
  path="/resident"
  element={
    <ProtectedRoute allowedRole="ApartmenResident" fetchUrl="http://localhost:5263/resident/GetUserRole">
      <ResidentDashboard />
    </ProtectedRoute>
  }
/>
      <Route
  path="/manager"
  element={
    <ProtectedRoute allowedRole="ApartmentManager" fetchUrl="http://localhost:5263/manager/GetUserRole">
      <ManagerDashboard />
    </ProtectedRoute>
  }
/>
      <Route path="*" element={<Navigate to="/login" />} />
    </Routes>
  )
}

export default App