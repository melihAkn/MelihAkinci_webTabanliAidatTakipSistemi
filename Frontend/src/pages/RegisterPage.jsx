import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

const RegisterPage = () => {
    const [name, setName] = useState('')
    const [surname, setSurName] = useState('')
    const [phoneNumber, setPhoneNumber] = useState('')
    const [email, setEmail] = useState('')
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const navigate = useNavigate()
    const handleRegister = async () => {
        try {
            const res = await fetch("http://localhost:5263/Home/register", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    "name": name,
                    "surname": surname,
                    "phoneNumber": phoneNumber,
                    "email": email,
                    "username": username,
                    "password": password
                })
            })
            const data = await res.json()
            if (!res.ok) {
                console.log(data)
                return
            }
            console.log(data)
            navigate("/login")
        }
        catch (err) {
            console.log(err)
        }
    }

    return (
        <div>
            <h2>yönetici kayıt olma</h2>

            <input placeholder="İsim" value={name} onChange={(e) => setName(e.target.value)} /><br />
            <input placeholder="Soyisim" value={surname} onChange={(e) => setSurName(e.target.value)} /><br />
            <input placeholder="Telefon Numarası" value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} /><br />
            <input placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} /><br />

            <input placeholder="Kullanıcı adı" value={username} onChange={(e) => setUsername(e.target.value)} /><br />
            <input placeholder="Şifre" type="password" value={password} onChange={(e) => setPassword(e.target.value)} /><br />

            <button onClick={handleRegister}>Kayıt Ol</button>

        </div>
    )
}

export default RegisterPage