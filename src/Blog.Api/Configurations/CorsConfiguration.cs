namespace Blog.Api.Configurations
{
    public static class CorsConfiguration
    {
        public static WebApplicationBuilder AddCorsConfig(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Development", builder =>
                            builder
                                .WithOrigins(configuration.GetSection("AllowedHosts").Get<string>())
                                .AllowAnyMethod()
                                .AllowAnyHeader());

                options.AddPolicy("Production", builder =>
                            builder
                                .WithOrigins(configuration.GetSection("AllowedHosts").Get<string>())
                                .SetIsOriginAllowedToAllowWildcardSubdomains()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
            });

            return builder;
        }
    }
}
