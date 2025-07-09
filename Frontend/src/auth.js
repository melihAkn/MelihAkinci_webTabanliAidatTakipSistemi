// Sahte bir auth kontrolü (gerçek sistemde token'dan rol alınır)

export function getRole() {
  return localStorage.getItem('userRoleId')
}

export function logout() {
  localStorage.removeItem('userRoleId')
}