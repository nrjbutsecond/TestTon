import { Link } from 'react-router-dom';
import { Calendar, MapPin, Star, ArrowRight, Users } from 'lucide-react';

const HomePage = () => {
  return (
    <div>
      {/* Hero Section */}
      <section className="relative bg-gradient-to-br from-black via-gray-900 to-primary min-h-[80vh] flex items-center">
        <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCI+PGNpcmNsZSBjeD0iMSIgY3k9IjEiIHI9IjEiIGZpbGw9IndoaXRlIiBmaWxsLW9wYWNpdHk9IjAuMDUiLz48L3N2Zz4=')] opacity-20"></div>

        <div className="container mx-auto px-4 relative z-10">
          <div className="max-w-3xl">
            <h1 className="text-5xl md:text-7xl font-bold text-white mb-6">
              <span className="text-primary">TEDx</span> Organizer Network
            </h1>
            <p className="text-xl md:text-2xl text-gray-300 mb-8">
              Connecting and enhancing the quality of TEDx events
            </p>
            <div className="flex flex-wrap gap-4">
              <Link to="/services" className="btn-primary inline-flex items-center gap-2">
                Explore Services <ArrowRight className="w-4 h-4" />
              </Link>
              <Link to="/about" className="btn-secondary inline-flex items-center gap-2">
                Watch Our Story
              </Link>
            </div>
          </div>
        </div>
      </section>

      {/* Why Choose TON */}
      <section className="py-20 bg-white">
        <div className="container mx-auto px-4">
          <div className="text-center mb-12">
            <h2 className="text-3xl font-bold mb-4">Why Choose TON?</h2>
            <p className="text-gray-600">Professional support and resources for TEDx organizers you need to succeed</p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
            {[
              {
                title: 'Supportive TEDx Organizers',
                description: 'Connect with experienced organizers worldwide',
                icon: Users
              },
              {
                title: 'A Responsive Community',
                description: 'Get quick help from our active community',
                icon: Users
              },
              {
                title: 'Shared support',
                description: 'Access resources and best practices',
                icon: Star
              },
              {
                title: 'Flexible plans',
                description: 'Choose services that fit your needs',
                icon: Calendar
              }
            ].map((feature, index) => (
              <div key={index} className="card p-6 text-center hover:shadow-xl transition-shadow">
                <feature.icon className="w-12 h-12 text-primary mx-auto mb-4" />
                <h3 className="font-semibold mb-2">{feature.title}</h3>
                <p className="text-sm text-gray-600">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Upcoming Events */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="flex justify-between items-center mb-12">
            <h2 className="text-3xl font-bold">Upcoming TEDx Events</h2>
            <Link to="/events" className="text-primary hover:underline flex items-center gap-2">
              View All Events <ArrowRight className="w-4 h-4" />
            </Link>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {[1, 2, 3].map((i) => (
              <div key={i} className="card hover:shadow-xl transition-shadow">
                <div className="relative">
                  <img
                    src={`https://via.placeholder.com/400x250?text=Event+${i}`}
                    alt="Event"
                    className="w-full h-48 object-cover"
                  />
                  <span className="absolute top-4 left-4 badge badge-popular">On Sale</span>
                </div>
                <div className="p-6">
                  <div className="flex items-center gap-2 text-sm text-gray-600 mb-2">
                    <Calendar className="w-4 h-4" />
                    <span>Mon, Apr 15, 2024</span>
                  </div>
                  <h3 className="font-semibold text-lg mb-2">TEDxHoChiMinhCity: Future of Innovation</h3>
                  <div className="flex items-center gap-2 text-sm text-gray-600 mb-4">
                    <MapPin className="w-4 h-4" />
                    <span>Saigon Convention Center, HCMC</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="text-primary font-bold">From 149.000 VND</span>
                    <Link to={`/events/${i}`} className="btn-primary text-sm py-2 px-4">
                      Get Tickets
                    </Link>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Partners */}
      <section className="py-20 bg-white">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl font-bold text-center mb-12">Our Partners</h2>
          <div className="grid grid-cols-2 md:grid-cols-5 gap-8 items-center">
            {[1, 2, 3, 4, 5].map((i) => (
              <div key={i} className="flex items-center justify-center p-4 grayscale hover:grayscale-0 transition-all">
                <img
                  src={`https://via.placeholder.com/150x80?text=Partner+${i}`}
                  alt={`Partner ${i}`}
                  className="max-w-full h-auto"
                />
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Merchandise Preview */}
      <section className="py-20 bg-gray-50">
        <div className="container mx-auto px-4">
          <div className="flex justify-between items-center mb-12">
            <h2 className="text-3xl font-bold">Merchandise</h2>
            <Link to="/merchandise" className="text-primary hover:underline flex items-center gap-2">
              View All Products <ArrowRight className="w-4 h-4" />
            </Link>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-6">
            {[1, 2, 3, 4].map((i) => (
              <div key={i} className="card hover:shadow-xl transition-shadow">
                <div className="relative">
                  <img
                    src={`https://via.placeholder.com/300x300?text=Product+${i}`}
                    alt="Product"
                    className="w-full aspect-square object-cover"
                  />
                  <span className="absolute top-4 left-4 badge bg-red-500 text-white">-20%</span>
                </div>
                <div className="p-4">
                  <h3 className="font-semibold mb-2">TON Official T-Shirt</h3>
                  <div className="flex items-center gap-2 mb-2">
                    <div className="flex text-yellow-400">
                      {[...Array(5)].map((_, i) => (
                        <Star key={i} className="w-4 h-4 fill-current" />
                      ))}
                    </div>
                    <span className="text-sm text-gray-600">(234)</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="text-primary font-bold">149.000 VND</span>
                    <span className="text-sm text-gray-400 line-through">199.000 VND</span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>
    </div>
  );
};

export default HomePage;
