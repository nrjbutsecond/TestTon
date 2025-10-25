import { Link } from 'react-router-dom';
import { Search, ShoppingCart, User } from 'lucide-react';
import { useState } from 'react';

const Header = () => {
  const [searchQuery, setSearchQuery] = useState('');

  return (
    <header className="bg-black text-white sticky top-0 z-50">
      <div className="container mx-auto px-4 py-4">
        <div className="flex items-center justify-between gap-4">
          {/* Logo */}
          <Link to="/" className="flex items-center space-x-2">
            <div className="text-2xl font-bold">
              <span className="text-primary">TED</span>
              <span className="text-white">x</span>
            </div>
            <div className="text-xs text-gray-400">
              <div>ORGANIZER</div>
              <div>NETWORK</div>
            </div>
          </Link>

          {/* Search Bar */}
          <div className="flex-1 max-w-md">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-gray-400" />
              <input
                type="text"
                placeholder="Search events, services,..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full bg-gray-900 text-white pl-10 pr-4 py-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
          </div>

          {/* Navigation */}
          <nav className="hidden md:flex items-center space-x-6 text-sm">
            <Link to="/services" className="hover:text-primary transition-colors">
              Services
            </Link>
            <Link to="/partners" className="hover:text-primary transition-colors">
              Partners
            </Link>
            <Link to="/tedx-vietnam" className="hover:text-primary transition-colors">
              TEDx In Vietnam
            </Link>
            <Link to="/merchandise" className="hover:text-primary transition-colors">
              Merchandises
            </Link>
            <Link to="/tickets" className="hover:text-primary transition-colors">
              Tickets
            </Link>
          </nav>

          {/* Icons */}
          <div className="flex items-center space-x-4">
            <Link to="/cart" className="hover:text-primary transition-colors">
              <ShoppingCart className="w-5 h-5" />
            </Link>
            <Link to="/profile" className="hover:text-primary transition-colors">
              <User className="w-5 h-5" />
            </Link>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
