using Blog.Application.Exceptions;
using Blog.Application.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Blog.Api.Middlewares
{
    public static class ExceptionMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    ProblemDetails error;
                    HttpStatusCode errorCode = HttpStatusCode.InternalServerError;

                    var logger = loggerFactory.CreateLogger("ExceptionHandler");

                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerFeature != null)
                    {
                        if (exceptionHandlerFeature.Error is BusinessException businessException)
                        {
                            logger.LogError($"Erro de negócio no servidor: {businessException.AgruparTodasAsMensagens()}");

                            errorCode = HttpStatusCode.BadRequest;

                            error = new ProblemDetails()
                            {
                                Title = "Erro de negócio",
                                Status = StatusCodes.Status400BadRequest,
                                Detail = businessException.Message,
                                Instance = context.Request.HttpContext.Request.Path,
                            };
                        }
                        else
                        {

                            logger.LogCritical($"Erro interno do servidor: {exceptionHandlerFeature.Error.AgruparTodasAsMensagens()}");

                            var mensagemErro = env.IsProduction()
                                ? "Erro interno no servidor, contate o suporte"
                                : exceptionHandlerFeature.Error.AgruparTodasAsMensagens();

                            error = new ProblemDetails()
                            {
                                Title = "Erro interno no servidor",
                                Status = StatusCodes.Status500InternalServerError,
                                Detail = mensagemErro,
                                Instance = context.Request.HttpContext.Request.Path,
                            };
                        }
                    }
                    else
                    {
                        logger.LogCritical($"Erro inesperado não encontrou a exception");

                        error = new ProblemDetails()
                        {
                            Title = "Erro inesperado no servidor",
                            Status = StatusCodes.Status500InternalServerError,
                            Detail = "Erro interno no servidor, contate o suporte",
                            Instance = context.Request.HttpContext.Request.Path,
                        };
                    }

                    context.Response.StatusCode = (int)errorCode;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonSerializer.Serialize(error));
                });
            });
        }
    }
}
