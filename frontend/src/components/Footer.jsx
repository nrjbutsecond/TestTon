import { Link } from 'react-router-dom';
import { Facebook, Instagram, Youtube, Linkedin, Twitter } from 'lucide-react';

const Footer = () => {
  return (
    <footer className="bg-black text-white py-12">
      <div className="container mx-auto px-4">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* Logo and Brand */}
          <div>
            <div className="flex items-center space-x-2 mb-4">
              <div className="text-2xl font-bold">
                <span className="text-primary">TED</span>
                <span className="text-white">x</span>
              </div>
              <div className="text-xs text-gray-400">
                <div>ORGANIZER</div>
                <div>NETWORK</div>
              </div>
            </div>
            <p className="text-gray-400 text-sm">
              Connecting and enhancing the quality of TEDx events
            </p>
            <div className="flex space-x-4 mt-4">
              <a href="#" className="text-gray-400 hover:text-primary transition-colors">
                <Facebook className="w-5 h-5" />
              </a>
              <a href="#" className="text-gray-400 hover:text-primary transition-colors">
                <Instagram className="w-5 h-5" />
              </a>
              <a href="#" className="text-gray-400 hover:text-primary transition-colors">
                <Youtube className="w-5 h-5" />
              </a>
              <a href="#" className="text-gray-400 hover:text-primary transition-colors">
                <Linkedin className="w-5 h-5" />
              </a>
              <a href="#" className="text-gray-400 hover:text-primary transition-colors">
                <Twitter className="w-5 h-5" />
              </a>
            </div>
          </div>

          {/* Services */}
          <div>
            <h3 className="font-semibold mb-4">Services</h3>
            <ul className="space-y-2 text-sm text-gray-400">
              <li>
                <Link to="/services/basic" className="hover:text-white transition-colors">
                  Basic
                </Link>
              </li>
              <li>
                <Link to="/services/event-experience" className="hover:text-white transition-colors">
                  (plus) Event Experience
                </Link>
              </li>
              <li>
                <Link to="/services/after-party" className="hover:text-white transition-colors">
                  (plus) After Party
                </Link>
              </li>
              <li>
                <Link to="/services/wow" className="hover:text-white transition-colors">
                  WOW
                </Link>
              </li>
            </ul>
          </div>

          {/* Community */}
          <div>
            <h3 className="font-semibold mb-4">Community</h3>
            <ul className="space-y-2 text-sm text-gray-400">
              <li>
                <Link to="/organizations" className="hover:text-white transition-colors">
                  TEDx Organizations
                </Link>
              </li>
              <li>
                <Link to="/sponsors" className="hover:text-white transition-colors">
                  Sponsors
                </Link>
              </li>
              <li>
                <Link to="/events" className="hover:text-white transition-colors">
                  Events
                </Link>
              </li>
            </ul>
          </div>

          {/* Support */}
          <div>
            <h3 className="font-semibold mb-4">Support</h3>
            <ul className="space-y-2 text-sm text-gray-400">
              <li>
                <Link to="/faq" className="hover:text-white transition-colors">
                  FAQ
                </Link>
              </li>
              <li>
                <Link to="/contact" className="hover:text-white transition-colors">
                  Contact
                </Link>
              </li>
              <li>
                <Link to="/terms" className="hover:text-white transition-colors">
                  Terms of Service
                </Link>
              </li>
              <li>
                <Link to="/privacy" className="hover:text-white transition-colors">
                  Privacy Policy
                </Link>
              </li>
            </ul>
          </div>
        </div>

        <div className="border-t border-gray-800 mt-8 pt-8 text-center text-sm text-gray-400">
          © 2025 TEDx Organizer Network. All rights reserved
        </div>
      </div>
    </footer>
  );
};

export default Footer;
