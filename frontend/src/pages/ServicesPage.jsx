import { Link } from 'react-router-dom';
import { Star, Clock, Users } from 'lucide-react';

const ServicesPage = () => {
  const services = [
    {
      id: 1,
      name: 'Basic Package',
      category: 'Event Planning',
      price: '60.000.000 VND',
      duration: '3-5 months',
      people: '14-16 people',
      rating: 4.9,
      reviews: 127,
      popular: true,
      features: ['Venue sourcing', 'Technical setup', 'Day of coordination', '+1 more features']
    },
    {
      id: 2,
      name: 'Event Experience',
      category: 'Experience',
      price: '15.000.000 VND',
      duration: '3-5 months',
      people: 'Full team',
      rating: 4.7,
      reviews: 165,
      popular: true,
      features: ['Audience experience design', 'Interactive elements', 'Technology integration', '+1 more features']
    },
    {
      id: 3,
      name: 'Mentoring (Full)',
      category: 'Mentoring',
      price: '16.000.000 VND',
      duration: '3-6 months',
      people: '1 mentor',
      rating: 4.9,
      reviews: 113,
      popular: true,
      features: ['Weekly mentoring sessions', 'Strategic planning guidance', 'Best practices sharing', '+1 more features']
    },
  ];

  return (
    <div className="py-12 bg-gray-50 min-h-screen">
      <div className="container mx-auto px-4">
        <div className="mb-8">
          <h1 className="text-4xl font-bold mb-4">Services</h1>
          <div className="flex flex-wrap gap-4 items-center">
            <input
              type="text"
              placeholder="Search services..."
              className="flex-1 min-w-[300px] px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
            />
            <select className="px-4 py-2 border rounded-lg">
              <option>All Categories</option>
              <option>Event Planning</option>
              <option>Experience</option>
            </select>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {services.map((service) => (
            <div key={service.id} className="card hover:shadow-xl transition-shadow relative">
              {service.popular && (
                <span className="absolute top-4 right-4 badge badge-popular">Popular</span>
              )}
              <div className="p-6">
                <h3 className="text-xl font-bold mb-1">{service.name}</h3>
                <p className="text-sm text-gray-600 mb-4">{service.category}</p>
                <div className="text-2xl font-bold text-primary mb-4">{service.price}</div>
                <div className="flex gap-4 text-sm text-gray-600 mb-4">
                  <div className="flex items-center gap-1">
                    <Clock className="w-4 h-4" />
                    <span>{service.duration}</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <Users className="w-4 h-4" />
                    <span>{service.people}</span>
                  </div>
                </div>
                <ul className="space-y-1 mb-4">
                  {service.features.map((feature, index) => (
                    <li key={index} className="text-sm text-gray-600 flex gap-2">
                      <span className="text-green-500">✓</span>
                      <span>{feature}</span>
                    </li>
                  ))}
                </ul>
                <Link to={`/services/${service.id}`} className="btn-primary w-full text-center block">
                  Get Started
                </Link>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ServicesPage;
