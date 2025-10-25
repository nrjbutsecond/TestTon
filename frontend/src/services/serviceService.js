import api from './api';

const serviceService = {
  // Get all service plans
  getAllServicePlans: async () => {
    const response = await api.get('/serviceplans');
    return response.data;
  },

  // Get service plan by ID
  getServicePlanById: async (id) => {
    const response = await api.get(`/serviceplans/${id}`);
    return response.data;
  },

  // Subscribe to service plan
  subscribeToServicePlan: async (subscriptionData) => {
    const response = await api.post('/serviceplans/subscribe', subscriptionData);
    return response.data;
  },

  // Get user's subscriptions
  getUserSubscriptions: async () => {
    const response = await api.get('/serviceplans/my-subscriptions');
    return response.data;
  },

  // Request consultation
  requestConsultation: async (consultationData) => {
    const response = await api.post('/consultationrequests', consultationData);
    return response.data;
  },
};

export default serviceService;
