using Blog.Api.Configurations;
using Blog.Api.Middlewares;
using Blog.Business.Services;
using Blog.Data.Helpers;
using Blog.Identity.Interfaces;
using Blog.Identity.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiConfig();

builder.AddCorsConfig(builder.Configuration);

builder.AddDbContextConfig();

builder.AddApiSecurity();

builder.AddSwaggerConfig();

builder.Services.AddScoped<IUser, AspNetUser>();
builder.Services.AddScoped<PostService>();

var app = builder.Build();

var defaultCulture = new CultureInfo("pt-BR");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new[] { defaultCulture },
    SupportedUICultures = new[] { defaultCulture }
};

app.UseRequestLocalization(localizationOptions);

app.ConfigureExceptionHandler(app.Environment, app.Services.GetRequiredService<ILoggerFactory>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("Development");
}
else
    app.UseCors("Production");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseDbMigrationHelper();

app.Run();
