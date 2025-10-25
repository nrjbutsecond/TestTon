import api from './api';

const authService = {
  // Login
  login: async (email, password, rememberMe = false) => {
    const response = await api.post('/users/login', { email, password, rememberMe });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      localStorage.setItem('user', JSON.stringify(response.data.user));
    }
    return response.data;
  },

  // Register
  register: async (userData) => {
    const response = await api.post('/users/register', userData);
    return response.data;
  },

  // Logout
  logout: () => {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  },

  // Get current user
  getCurrentUser: () => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  },

  // Check if logged in
  isLoggedIn: () => {
    return !!localStorage.getItem('token');
  },

  // Confirm email
  confirmEmail: async (userId, token) => {
    const response = await api.get(`/users/confirm-email?userId=${userId}&token=${token}`);
    return response.data;
  },

  // Change password
  changePassword: async (oldPassword, newPassword) => {
    const response = await api.post('/users/change-password', { oldPassword, newPassword });
    return response.data;
  },

  // Get user profile
  getUserProfile: async (userId) => {
    const response = await api.get(`/users/${userId}`);
    return response.data;
  },

  // Update user profile
  updateUserProfile: async (userId, userData) => {
    const response = await api.put(`/users/${userId}`, userData);
    return response.data;
  },
};

export default authService;
