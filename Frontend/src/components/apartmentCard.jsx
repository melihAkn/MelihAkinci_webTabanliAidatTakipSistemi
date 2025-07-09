const Card = ({ data }) => {
  return (
    <div style={{
      border: '1px solid #ccc',
      borderRadius: '8px',
      padding: '1rem',
      marginBottom: '1rem',
      backgroundColor: '#fafafa',
      boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
    }}>
      {Object.entries(data).map(([key, value]) => (
        <p key={key}><strong>{key}:</strong> {String(value)}</p>
      ))}
    </div>
  )
}

export default Card