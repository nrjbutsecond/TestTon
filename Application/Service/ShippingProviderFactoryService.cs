using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service
{
    public class ShippingProviderFactoryService: IShippingProviderFactoryService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, Type> _providerTypes;

        public ShippingProviderFactoryService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _providerTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "local", typeof(LocalShippingProviderService) },
                { "ghn", typeof(GhnShippingProviderService) },
              //  { "ghtk", typeof(GhtkShippingProvider) },
                // Add more providers as needed
            };
        }

        public IShippingProviderService GetProvider(string providerName)
        {
            if (!_providerTypes.ContainsKey(providerName))
                throw new NotSupportedException($"Shipping provider '{providerName}' is not supported");

            var providerType = _providerTypes[providerName];
            return (IShippingProviderService)_serviceProvider.GetRequiredService(providerType);
        }

        public List<string> GetAvailableProviders()
        {
            var providers = new List<string>();

            foreach (var kvp in _providerTypes)
            {
                var provider = GetProvider(kvp.Key);
                if (provider.IsAvailable)
                    providers.Add(kvp.Key);
            }

            return providers;
        }

        public IShippingProviderService GetDefaultProvider()
        {
            var defaultProviderName = _configuration["Shipping:DefaultProvider"] ?? "local";
            return GetProvider(defaultProviderName);
        }
    }
}
