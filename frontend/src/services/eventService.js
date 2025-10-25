import api from './api';

const eventService = {
  // Get all events
  getAllEvents: async () => {
    const response = await api.get('/talkevent');
    return response.data;
  },

  // Get event by ID
  getEventById: async (id) => {
    const response = await api.get(`/talkevent/${id}`);
    return response.data;
  },

  // Create event (for organizers)
  createEvent: async (eventData) => {
    const response = await api.post('/talkevent', eventData);
    return response.data;
  },

  // Update event
  updateEvent: async (id, eventData) => {
    const response = await api.put(`/talkevent/${id}`, eventData);
    return response.data;
  },

  // Delete event
  deleteEvent: async (id) => {
    const response = await api.delete(`/talkevent/${id}`);
    return response.data;
  },

  // Get tickets for event
  getEventTickets: async (eventId) => {
    const response = await api.get(`/tickets/${eventId}`);
    return response.data;
  },

  // Get ticket types
  getTicketTypes: async () => {
    const response = await api.get('/tickettypes');
    return response.data;
  },

  // Purchase ticket
  purchaseTicket: async (ticketData) => {
    const response = await api.post('/tickets', ticketData);
    return response.data;
  },
};

export default eventService;
