using Blog.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Configurations
{
    public static class DatabaseConfiguration
    {
        public static WebApplicationBuilder AddDbContextConfig(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            return builder;
        }
    }
}
