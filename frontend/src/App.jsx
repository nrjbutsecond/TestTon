import { BrowserRouter, Routes, Route } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import HomePage from './pages/HomePage';
import ServicesPage from './pages/ServicesPage';
import EventsPage from './pages/EventsPage';
import MerchandisePage from './pages/MerchandisePage';
import PartnersPage from './pages/PartnersPage';
import CartPage from './pages/CartPage';
import CheckoutPage from './pages/CheckoutPage';
import ProfilePage from './pages/ProfilePage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainLayout />}>
          <Route index element={<HomePage />} />
          <Route path="services" element={<ServicesPage />} />
          <Route path="events" element={<EventsPage />} />
          <Route path="tedx-vietnam" element={<EventsPage />} />
          <Route path="merchandise" element={<MerchandisePage />} />
          <Route path="tickets" element={<EventsPage />} />
          <Route path="partners" element={<PartnersPage />} />
          <Route path="cart" element={<CartPage />} />
          <Route path="checkout" element={<CheckoutPage />} />
          <Route path="profile" element={<ProfilePage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
