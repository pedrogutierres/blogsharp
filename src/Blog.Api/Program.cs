using Blog.Api.Configurations;
using Blog.Identity.Interfaces;
using Blog.Identity.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiConfig();

builder.AddCorsConfig(builder.Configuration);

builder.AddDbContextConfig();

builder.AddApiSecurity();

builder.AddSwaggerConfig();

builder.Services.AddScoped<IUser, AspNetUser>();

var app = builder.Build();

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

app.Run();
