import { Routes, Route, Navigate } from 'react-router-dom'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'
import ResidentDashboard from './pages/residentDashboard/ResidentDashboard'
import ManagerDashboard from './pages/managerDashboard/ManagerDashboard'
import ProtectedRoute from './components/ProtectedRoute'

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path='/register' element={<RegisterPage />} />
      <Route
        path="/resident"
        element={
          <ProtectedRoute allowedRole="ApartmentResident" fetchUrl="http://localhost:5263/resident/get-user-role">
            <ResidentDashboard />
          </ProtectedRoute>
        }
      />
      <Route
        path="/manager"
        element={
          <ProtectedRoute allowedRole="ApartmentManager" fetchUrl="http://localhost:5263/manager/get-user-role">
            <ManagerDashboard />
          </ProtectedRoute>
        }
      />
      <Route path="*" element={<Navigate to="/login" />} />
    </Routes>
  )
}

export default App