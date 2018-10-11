using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiWrapper.Wrappers;

namespace WebApiWrapper
{
    public class WrapperHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (IsSwagger(request))
            {
                return await base.SendAsync(request, cancellationToken);
            }else
            {
                var response = await base.SendAsync(request, cancellationToken);
                return BuildApiResponse(request, response);
            }
            
        }

        private static HttpResponseMessage BuildApiResponse(HttpRequestMessage request,HttpResponseMessage response)
        {
            dynamic content = null;
            object data = null;
            string errorMessage = null;
            ApiError apiError = null;

            var code = (int)response.StatusCode;
            if(response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
            {
                HttpError error = content as HttpError;
                if(error != null)
                {
                    content = null;
                    if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        apiError = new ApiError("the uri not fount");
                    }else if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        apiError = new ApiError("the uri not contain the content");
                    }else
                    {
                        errorMessage = error.Message;
#if DEBUG
                        errorMessage = string.Concat(errorMessage, error.ExceptionMessage, error.StackTrace);
#endif
                    }
                    data = new ApiResponse((int)code, ResponseMessageEnum.Failure.ToString(), null, apiError);
                }else
                {
                    data = content;
                }
            }else
            {
                if(response.TryGetContentValue(out content))
                {
                    Type type;
                    type = content?.GetType();
                    if (type.Name.Equals("ApiResponse"))
                    {
                        response.StatusCode = Enum.Parse(typeof(HttpStatusCode), content.StatusCode.ToString());
                        data = content;
                    }else if (type.Name.Equals("SwaggerDocument"))
                    {
                        data = content;
                    }else
                    {
                        data = new ApiResponse(code, ResponseMessageEnum.Success.ToString(), content);
                    }
                }else
                {
                    if (response.IsSuccessStatusCode)
                    {
                        data = new ApiResponse((int)response.StatusCode, ResponseMessageEnum.Success.ToString());
                    }
                }
            }

            var newResponse = request.CreateResponse(response.StatusCode, data);
            foreach(var header in response.Headers)
            {
                newResponse.Headers.Add(header.Key, header.Value);
            }
            return newResponse;
        }

        private bool IsSwagger(HttpRequestMessage request)
        {
            return request.RequestUri.PathAndQuery.StartsWith("/swagger");
        }
    }
}
