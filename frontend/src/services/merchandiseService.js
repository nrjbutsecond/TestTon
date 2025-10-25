import api from './api';

const merchandiseService = {
  // Get all merchandise
  getAllMerchandise: async () => {
    const response = await api.get('/merchandise');
    return response.data;
  },

  // Get merchandise by ID
  getMerchandiseById: async (id) => {
    const response = await api.get(`/merchandise/${id}`);
    return response.data;
  },

  // Search merchandise
  searchMerchandise: async (searchParams) => {
    const response = await api.post('/merchandise/search', searchParams);
    return response.data;
  },

  // Get merchandise variants
  getMerchandiseVariants: async (merchandiseId) => {
    const response = await api.get(`/merchandise/${merchandiseId}/variants`);
    return response.data;
  },

  // Get reviews for merchandise
  getReviews: async (merchandiseId) => {
    const response = await api.get(`/reviews/merchandise/${merchandiseId}`);
    return response.data;
  },

  // Create review
  createReview: async (reviewData) => {
    const response = await api.post('/reviews', reviewData);
    return response.data;
  },
};

export default merchandiseService;
