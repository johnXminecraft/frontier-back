using FrontierWeb.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrontierWeb.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBlogInfrastructure(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<BlogDbContext>(opt =>
                opt.UseSqlite(cfg.GetConnectionString("BlogDb") ?? "Data Source=blog.db"));

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPostService, PostService>();

            return services;
        }
    }
}
