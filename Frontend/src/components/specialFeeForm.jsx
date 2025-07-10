import { useState } from "react"

const SpecialFeeForm = ({ residentId, onSubmit, onCancel }) => {
    const [name, setName] = useState('')
    const [amount, setAmount] = useState('')
    const [description, setDescription] = useState('')

    const handleSubmit = async (e) => {
        e.preventDefault()
        if (!amount || !description) {
            alert("Tüm alanları doldurun.")
            return
        }
        onSubmit({
            name,
            amount: parseFloat(amount),
            description,
            residentId
        })
    }

    return (
        <form onSubmit={handleSubmit} style={{ marginTop: '1rem' }}>
            <div>
                <input
                    type="text"
                    placeholder="borç ismi"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
                <input
                    type="number"
                    placeholder="Tutar"
                    value={amount}
                    onChange={(e) => setAmount(e.target.value)}
                />
            </div>
            <div>
                <input
                    type="text"
                    placeholder="Açıklama"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
            </div>
            <button type="submit">Gönder</button>
            <button type="button" onClick={onCancel} style={{ marginLeft: '0.5rem' }}>
                İptal
            </button>
        </form>
    )
}

export default SpecialFeeForm
