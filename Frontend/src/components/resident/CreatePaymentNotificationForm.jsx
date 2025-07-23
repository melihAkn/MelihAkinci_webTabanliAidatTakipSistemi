import { useState } from "react"

const CreatePaymentNotificationForm = ({ maintenanceFeeId = 0, specialFeeId = 0, onSubmit, onCancel }) => {
    const [notificationMessage, setNotificationMessage] = useState('')
    const [file, setFile] = useState(null)
    

    const handleSubmit = async (e) => {
        e.preventDefault()

        
        if (!notificationMessage) {
            alert("Tüm alanları doldurun.")
            return
        }
        onSubmit({
            maintenanceFeeId,
            specialFeeId,
            notificationMessage,
            file
        })
    }

    return (
        <form onSubmit={handleSubmit} style={{ marginTop: '1rem' }}>
            <div>
                
                <input
                    type="text"
                    placeholder="ödeme bildirimi mesajı"
                    onChange={(e) => setNotificationMessage(e.target.value)}
                />

                <input
                    type="file"
                    onChange={(e) => setFile(e.target.files[0])}
                />
            </div>
            <div>
            </div>
            <button type="submit">Gönder</button>
            <button type="button" onClick={onCancel} style={{ marginLeft: '0.5rem' }}>
                İptal
            </button>
        </form>
    )
}

export default CreatePaymentNotificationForm
