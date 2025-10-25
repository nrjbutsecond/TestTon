import api from './api';

const organizationService = {
  // Get all organizations
  getAllOrganizations: async () => {
    const response = await api.get('/organization');
    return response.data;
  },

  // Get organization by ID
  getOrganizationById: async (id) => {
    const response = await api.get(`/organization/${id}`);
    return response.data;
  },

  // Create organization
  createOrganization: async (orgData) => {
    const response = await api.post('/organization', orgData);
    return response.data;
  },

  // Update organization
  updateOrganization: async (id, orgData) => {
    const response = await api.put(`/organization/${id}`, orgData);
    return response.data;
  },

  // Get organization statistics
  getOrganizationStats: async (id) => {
    const response = await api.get(`/organization/${id}/statistics`);
    return response.data;
  },

  // Apply for partnership
  applyForPartnership: async (applicationData) => {
    const response = await api.post('/organization/partnership/apply', applicationData);
    return response.data;
  },
};

export default organizationService;
