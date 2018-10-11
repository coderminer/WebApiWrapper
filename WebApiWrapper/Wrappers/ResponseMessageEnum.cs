using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiWrapper.Wrappers
{
    public enum ResponseMessageEnum
    {
        [Description("Request successful.")]
        Success,
        [Description("Request responsed with exception.")]
        Exception,
        [Description("Request denied.")]
        Unauthorized,
        [Description("Request with validation error.")]
        ValidationError,
        [Description("Unable to process the request.")]
        Failure
    }
}
