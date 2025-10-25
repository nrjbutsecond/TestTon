import api from './api';

const orderService = {
  // Create order
  createOrder: async (orderData) => {
    const response = await api.post('/order', orderData);
    return response.data;
  },

  // Get user's orders
  getMyOrders: async () => {
    const response = await api.get('/order/my');
    return response.data;
  },

  // Get order by ID
  getOrderById: async (id) => {
    const response = await api.get(`/order/${id}`);
    return response.data;
  },

  // Cancel order
  cancelOrder: async (id) => {
    const response = await api.post(`/order/${id}/cancel`);
    return response.data;
  },

  // Update order status (admin)
  updateOrderStatus: async (id, status) => {
    const response = await api.put(`/order/${id}/status`, { status });
    return response.data;
  },
};

export default orderService;
