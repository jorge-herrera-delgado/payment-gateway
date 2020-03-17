using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace payment_gateway.Helper
{
    internal class ErrorResponse
    {
        public static string GetErrorDetail(ErrorCode errorCode)
        {
            var dic = new Dictionary<ErrorCode, string>
            {
                {
                    ErrorCode.HasLogicError,
                    "There has been error processing your request. Please, check 'data' object for further details."
                }
            };

            return !dic.ContainsKey(errorCode)
                ? "Error no defined, please contact the administrator for further investigation."
                : dic[errorCode];
        }
    }

    internal enum ErrorCode
    {
        HasLogicError = 301,
        HasRequestError = 305
    }
}
