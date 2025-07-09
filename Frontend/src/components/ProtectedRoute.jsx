import { useEffect, useState } from 'react';
import { Navigate } from 'react-router-dom';

const ProtectedRoute = ({ allowedRole, fetchUrl, children }) => {
  const [loading, setLoading] = useState(true);
  const [authorized, setAuthorized] = useState(false);

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const res = await fetch(fetchUrl, {
          credentials: 'include'
        });

        if (!res.ok) {
          setAuthorized(false);
        } else {
          const data = await res.json();
          setAuthorized(data.userRole === allowedRole);
        }
      } catch {
        setAuthorized(false);
      } finally {
        setLoading(false);
      }
    };

    checkAuth();
  }, [allowedRole, fetchUrl]);

  if (loading) return <div>Yükleniyor...</div>;
  if (!authorized) return <Navigate to="/" />;

  return children;
};

export default ProtectedRoute;