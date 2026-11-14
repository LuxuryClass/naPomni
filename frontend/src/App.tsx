import { Navigate, Route, Routes } from 'react-router-dom';
import { MainPage } from './pages/MainPage/MainPage';
import { ROUTES } from './routes';

export default function App() {
  return (
    <Routes>
      <Route path={ROUTES.main} element={<MainPage />} />
      <Route path="*" element={<Navigate to={ROUTES.main} replace />} />
    </Routes>
  );
}