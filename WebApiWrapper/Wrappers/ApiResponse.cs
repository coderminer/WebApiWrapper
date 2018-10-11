using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiWrapper.Wrappers
{
    public class ApiResponse
    {
        public string Version { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public ApiError ResponseException { get; set; }
        public object Result { get; set; }

        public ApiResponse(int statusCode,string message = "",object result = null,ApiError apiError = null,string apiVersion = "1.0.0")
        {
            this.StatusCode = statusCode;
            this.Message = message;
            this.Result = result;
            this.ResponseException = apiError;
            this.Version = apiVersion;
        }
    }
}
