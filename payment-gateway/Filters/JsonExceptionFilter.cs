using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using payment_gateway.Model;

namespace payment_gateway.Filters
{
    public class JsonExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;

        public JsonExceptionFilter(IWebHostEnvironment env)
        {
            _env = env;
        }
        public void OnException(ExceptionContext context)
        {
            var error = new ResponseModel
            {
                Success = false,
                TotalRecords = 0
            };

            if (_env.IsDevelopment())
            {
                error.ErrorMessage = context.Exception.Message;
                error.ErrorCode = "500";
                error.ErrorStackTrace = context.Exception.StackTrace;
            }
            else
            {
                error.ErrorMessage = context.Exception.Message;
                error.ErrorCode = "500";
            }

            context.Result = new ObjectResult(error)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
