import { useState } from "react"

const AddApartmentResidentForm = ({ apartmentUnitId, onSubmit, onCancel }) => {
    const [name, setName] = useState('')
    const [surname, setSurName] = useState('')
    const [email, setEmail] = useState('')
    const [phoneNumber, setPhoneNumber] = useState('')

    const handleSubmit = async (e) => {
        e.preventDefault()
        if (!name || !surname || !email || !phoneNumber) {
            alert("Tüm alanları doldurun.")
            return
        }
        onSubmit({
            name,
            surname,
            email,
            phoneNumber,
            apartmentUnitId
        })
    }

    return (
        <form onSubmit={handleSubmit} style={{ marginTop: '1rem' }}>
            <div>
                <div>
                    <input
                        type="text"
                        placeholder="apartman sakini ismi"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                    <input
                        type="text"
                        placeholder="apartman sakini soy ismi"
                        value={surname}
                        onChange={(e) => setSurName(e.target.value)}
                    />
                </div>

                <div>
                    <input
                        type="text"
                        placeholder="apartman sakini telefon numarası"
                        value={phoneNumber}
                        onChange={(e) => setPhoneNumber(e.target.value)}
                    />
                    <input
                        type="email"
                        placeholder="apartman sakini mail"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </div>

            </div>

            <button type="submit">Gönder</button>
            <button type="button" onClick={onCancel} style={{ marginLeft: '0.5rem' }}>
                İptal
            </button>
        </form>
    )
}

export default AddApartmentResidentForm
