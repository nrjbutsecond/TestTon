using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IShippingProviderFactoryService
    {
        IShippingProviderService GetProvider(string providerName);
        List<string> GetAvailableProviders();
        IShippingProviderService GetDefaultProvider();
    }
}
