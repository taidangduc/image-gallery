import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { StoragePage } from './features/Storage'

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<StoragePage />} />
      </Routes>
    </Router>
  )
}

export default App
