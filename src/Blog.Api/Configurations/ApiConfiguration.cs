namespace Blog.Api.Configurations
{
    public static class ApiConfiguration
    {
        public static WebApplicationBuilder AddApiConfig(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            return builder;
        }
    }
}
