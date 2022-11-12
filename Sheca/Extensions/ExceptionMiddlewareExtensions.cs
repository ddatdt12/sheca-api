using Sheca.Error;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Sheca.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app,
       ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                    {
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";

                            if (contextFeature != null)
                        {
                            logger.LogError($"Something went wrong: {contextFeature.Error}");

                            var errorDetail = contextFeature.Error as ApiException;
                            string message = contextFeature.Error.Message ?? "Internal Server Error.";

                            if (errorDetail != null)
                            {
                                context.Response.StatusCode = errorDetail.StatusCode;
                                message = errorDetail.Message;
                            }

                            if (contextFeature.Error is DbUpdateException updateErr)
                            {
                                message = updateErr?.InnerException?.Message ?? updateErr?.Message ?? "";
                            }

                            if (contextFeature?.Error?.InnerException != null)
                            {
                                message = contextFeature?.Error?.InnerException.Message ?? "";
                            }


                            Exception errorRes = new ApiException(message, statusCode: context.Response.StatusCode);
                            await context.Response.WriteAsync(errorRes.ToString());
                        }
                    });
            });
        }

    }
}
