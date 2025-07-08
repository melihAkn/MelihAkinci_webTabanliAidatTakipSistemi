// Sahte bir auth kontrolü (gerçek sistemde token'dan rol alınır)
export function login(username, password) {
  if (username === 'manager') {
    localStorage.setItem('role', 'manager')
    return true
  } else if (username === 'owner') {
    localStorage.setItem('role', 'owner')
    return true
  }
  return false
}

export function getRole() {
  return localStorage.getItem('role')
}

export function logout() {
  localStorage.removeItem('role')
}