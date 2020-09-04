using System.Collections.Generic;
using System.Linq;
using payment_gateway.Helper;
using payment_gateway.Mapper.Engine;
using payment_gateway.Model;
using payment_gateway_core.Validation.Engine;
using ErrorCode = payment_gateway.Helper.ErrorCode;

namespace payment_gateway.Mapper
{
    public class GlobalResultToResponse<T>
        where T : class

    {
        public ResponseModel MapToDestination(object source)
        {
            var response = new ResponseModel();
            if (source == null)
            {
                response.Success = false;
                response.Data = null;
                response.TotalRecords = 0;
            }
            else if (source.GetType() != typeof(Result))
            {
                var mapper = MapperModelFactory<T, object>.GetMapper();
                var result = source as T;
                var model = mapper.MapToDestination(result);
                response.Data = model;
            }
            else
            {
                var result = source as Result;
                response.Data = result;
                response.Success = false;
                response.ErrorMessage = ErrorResponse.GetErrorDetail(ErrorCode.HasLogicError);
                response.ErrorCode = ((int)ErrorCode.HasLogicError).ToString();
            }

            return response;
        }

        public ResponseModel MapToDestination<TCollectionType>(object source)
        {
            var response = new ResponseModel();
            if (source == null)
            {
                response.Success = false;
                response.Data = null;
                response.TotalRecords = 0;
            }
            else if (source.GetType() != typeof(Result))
            {
                var mapper = MapperModelFactory<T, object>.GetMapper();
                var result = source as T;
                var model = mapper.MapToDestination(result);
                response.Data = model;

                var genericType = model.GetType().GetGenericTypeDefinition();
                if (genericType == typeof(List<>) || genericType == typeof(IEnumerable<>))
                    response.TotalRecords = (model as IEnumerable<TCollectionType> ?? new List<TCollectionType>()).Count();
            }
            else
            {
                var result = source as Result;
                response.Data = result;
                response.Success = false;
                response.ErrorMessage = ErrorResponse.GetErrorDetail(ErrorCode.HasLogicError);
                response.ErrorCode = ((int)ErrorCode.HasLogicError).ToString();
            }

            return response;
        }
    }
}
