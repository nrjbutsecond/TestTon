using Domain.Interface;
using Infrastructure.Repo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticket.Infrastructure.Data;
using Ticket.Infrastructure.Repo;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

            // Add repositories
            services.AddScoped(typeof(IRepo<>), typeof(Repo<>));
            services.AddScoped<IArticleRepo, ArticleRepo>();
            // ... other repository registrations ...

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}