using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using WebApiWrapper.Wrappers;
using System.Net.Http;

namespace WebApiWrapper.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            ApiError apiError = null;
            ApiResponse apiResponse = null;
            int code = 0;

            if(context.Exception is ApiException)
            {
                var ex = context.Exception as ApiException;
                apiError = new ApiError(ex.Message);
                apiError.ValidationErrors = ex.Errors;
                apiError.ReferenceErrorCode = ex.ReferenceErrorCode;
                apiError.ReferenceDocumentLink = ex.ReferenceDocumentLink;
                code = ex.StatusCode;
            }else if(context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiError("Unauthorized Access");
                code = (int)HttpStatusCode.Unauthorized;
            }else
            {
#if !DEBUG
                var msg = "an unhandled error";
                string stack = null;
#else
                var msg = context.Exception.GetBaseException().Message;
                string stack = context.Exception.StackTrace;
#endif
                apiError = new ApiError(msg);
                apiError.Details = stack;
                code = (int)HttpStatusCode.InternalServerError;
            }

            apiResponse = new ApiResponse(code, ResponseMessageEnum.Exception.ToString(), null, apiError);
            HttpStatusCode c = (HttpStatusCode)code;
            context.Response = context.Request.CreateResponse(c, apiResponse);
        }
    }
}
