using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Model;

namespace Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse();
            int statusCode = (int)HttpStatusCode.InternalServerError;

            switch (exception)
            {
                case MongoException mtex:
                case TimeoutException ttex:
                    statusCode = (int)HttpStatusCode.ServiceUnavailable;
                    response.Code = "DB_UNAVAILABLE";
                    response.Message = "Serviço de banco de dados indisponível. Tente novamente mais tarde.";
                    response.Details = exception.Message;
                    break;
                case UnauthorizedAccessException _:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    response.Code = "UNAUTHORIZED";
                    response.Message = "Acesso não autorizado.";
                    response.Details = exception.Message;
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    response.Code = "INTERNAL_ERROR";
                    response.Message = "Ocorreu um erro no servidor.";
                    response.Details = exception.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;
            var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(response, opts);
            return context.Response.WriteAsync(payload);
        }
    }
}
