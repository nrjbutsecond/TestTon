import { useState } from 'react';
import { Calendar, MapPin, Users, Star, Filter, Search } from 'lucide-react';
import { Link } from 'react-router-dom';

const EventsPage = () => {
  const [selectedCategory, setSelectedCategory] = useState('All Events');
  const [searchQuery, setSearchQuery] = useState('');

  const categories = ['All Events', 'Featured', 'On Sale', 'Coming Soon', 'Early Bird'];

  const events = [
    {
      id: 1,
      title: 'TEDxHoChiMinhCity: Innovation',
      organizer: 'TEDxHoChiMinhCity',
      date: 'Mon, Apr 15, 2024',
      time: '6:00 PM',
      location: 'Saigon Convention Center, Ho Chi Minh City, Vietnam',
      category: 'Technology',
      rating: 4.9,
      attendees: '850/1000 attending',
      price: 'From 149.000 VND',
      originalPrice: null,
      image: 'https://via.placeholder.com/400x250?text=Event+1',
      status: 'On Sale',
      tags: ['Innovation', 'AI', 'Startup']
    },
    {
      id: 2,
      title: 'TEDxHanoi: Sustainable Future',
      organizer: 'TEDxHanoi',
      date: 'Sat, Apr 20, 2024',
      time: '2:00 PM',
      location: 'National Convention Center, Hanoi, Vietnam',
      category: 'Environment',
      rating: 4.6,
      attendees: '450/800 attending',
      price: 'From 49.000 VND',
      originalPrice: null,
      image: 'https://via.placeholder.com/400x250?text=Event+2',
      status: 'On Discount',
      tags: ['Environment', 'Sustainability', 'Climate']
    },
    {
      id: 3,
      title: 'TEDxBusinessSummit: Digital',
      organizer: 'TEDxBusiness',
      date: 'Thu, Apr 25, 2024',
      time: '9:00 AM',
      location: 'Grand Hotel Conference Hall, Singapore',
      category: 'Business',
      rating: 4.8,
      attendees: '600/800 attending',
      price: 'Sold Out',
      originalPrice: null,
      image: 'https://via.placeholder.com/400x250?text=Event+3',
      status: 'Sold Out',
      tags: ['Business', 'Digital', 'Leadership']
    },
    {
      id: 4,
      title: 'TEDx Youth: Future Leaders',
      organizer: 'TEDxYouth',
      date: 'Sun, May 4, 2024',
      time: '10:00 AM',
      location: 'Youth Cultural Center, Da Nang, Vietnam',
      category: 'Education',
      rating: 4.7,
      attendees: '0/500 attending',
      price: 'From 210.000 VND',
      originalPrice: null,
      image: 'https://via.placeholder.com/400x250?text=Event+4',
      status: 'Coming Soon',
      tags: ['Youth', 'Leadership', 'Education']
    },
  ];

  const filteredEvents = events.filter(event => {
    const matchesCategory = selectedCategory === 'All Events' || event.status === selectedCategory;
    const matchesSearch = event.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
                         event.organizer.toLowerCase().includes(searchQuery.toLowerCase());
    return matchesCategory && matchesSearch;
  });

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header Section */}
      <div className="bg-white border-b">
        <div className="container mx-auto px-4 py-6">
          <h1 className="text-3xl font-bold mb-6">TEDx Events</h1>

          {/* Search and Filters */}
          <div className="flex flex-wrap gap-4 items-center">
            <div className="flex-1 min-w-[300px] relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                type="text"
                placeholder="Search events, speakers, topics..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>

            <select className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary">
              <option>All Categories</option>
              <option>Technology</option>
              <option>Business</option>
              <option>Environment</option>
              <option>Education</option>
            </select>

            <select className="px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary">
              <option>All Locations</option>
              <option>Ho Chi Minh City</option>
              <option>Hanoi</option>
              <option>Da Nang</option>
            </select>

            <button className="flex items-center gap-2 px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50">
              <Calendar className="w-4 h-4" />
              Pick a date
            </button>

            <button className="px-4 py-2 bg-primary text-white rounded-lg hover:bg-primary-dark">
              <Filter className="w-4 h-4" />
            </button>

            <button className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50">
              Event Date
            </button>
          </div>
        </div>
      </div>

      {/* Category Tabs */}
      <div className="bg-white border-b">
        <div className="container mx-auto px-4">
          <div className="flex gap-6 overflow-x-auto">
            {categories.map((category) => (
              <button
                key={category}
                onClick={() => setSelectedCategory(category)}
                className={`py-4 px-2 border-b-2 whitespace-nowrap transition-colors ${
                  selectedCategory === category
                    ? 'border-primary text-primary font-semibold'
                    : 'border-transparent text-gray-600 hover:text-gray-900'
                }`}
              >
                {category}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Events Grid */}
      <div className="container mx-auto px-4 py-8">
        <div className="mb-4 text-gray-600">
          Showing {filteredEvents.length} of {events.length} events
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {filteredEvents.map((event) => (
            <div key={event.id} className="card hover:shadow-xl transition-shadow">
              {/* Event Image */}
              <div className="relative">
                <img
                  src={event.image}
                  alt={event.title}
                  className="w-full h-48 object-cover"
                />
                <span className={`absolute top-4 left-4 badge ${
                  event.status === 'On Sale' ? 'badge-popular' :
                  event.status === 'Coming Soon' ? 'badge-status-planning' :
                  event.status === 'Sold Out' ? 'bg-gray-500 text-white' :
                  'bg-red-500 text-white'
                }`}>
                  {event.status}
                </span>
                {event.rating && (
                  <div className="absolute top-4 right-4 bg-white px-2 py-1 rounded-lg flex items-center gap-1">
                    <Star className="w-4 h-4 text-yellow-400 fill-current" />
                    <span className="text-sm font-semibold">{event.rating}</span>
                  </div>
                )}
              </div>

              {/* Event Info */}
              <div className="p-5">
                <div className="mb-3">
                  <span className="inline-block px-3 py-1 bg-blue-100 text-blue-700 text-xs rounded-full mb-2">
                    {event.category}
                  </span>
                  <h3 className="font-bold text-lg mb-1">{event.title}</h3>
                  <p className="text-sm text-gray-600">by {event.organizer}</p>
                </div>

                <div className="space-y-2 mb-4">
                  <div className="flex items-start gap-2 text-sm text-gray-600">
                    <Calendar className="w-4 h-4 mt-0.5 flex-shrink-0" />
                    <span>{event.date} • {event.time}</span>
                  </div>
                  <div className="flex items-start gap-2 text-sm text-gray-600">
                    <MapPin className="w-4 h-4 mt-0.5 flex-shrink-0" />
                    <span>{event.location}</span>
                  </div>
                  {event.attendees && (
                    <div className="flex items-center gap-2 text-sm text-gray-600">
                      <Users className="w-4 h-4 flex-shrink-0" />
                      <span>{event.attendees}</span>
                    </div>
                  )}
                </div>

                {/* Tags */}
                <div className="flex flex-wrap gap-2 mb-4">
                  {event.tags.map((tag, index) => (
                    <span key={index} className="text-xs bg-gray-100 text-gray-700 px-2 py-1 rounded">
                      {tag}
                    </span>
                  ))}
                </div>

                {/* Price and CTA */}
                <div className="flex items-center justify-between pt-4 border-t">
                  <div>
                    <span className="text-primary font-bold">{event.price}</span>
                    {event.originalPrice && (
                      <span className="text-sm text-gray-400 line-through ml-2">
                        {event.originalPrice}
                      </span>
                    )}
                  </div>
                  <Link
                    to={`/events/${event.id}`}
                    className={`px-4 py-2 rounded-lg font-semibold transition-colors ${
                      event.status === 'Sold Out'
                        ? 'bg-gray-300 text-gray-600 cursor-not-allowed'
                        : 'bg-primary text-white hover:bg-primary-dark'
                    }`}
                  >
                    {event.status === 'Sold Out' ? 'Sold Out' :
                     event.status === 'Coming Soon' ? 'Notify Me' : 'Get Tickets'}
                  </Link>
                </div>
              </div>
            </div>
          ))}
        </div>

        {filteredEvents.length === 0 && (
          <div className="text-center py-12">
            <p className="text-gray-500 text-lg">No events found matching your criteria</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default EventsPage;
