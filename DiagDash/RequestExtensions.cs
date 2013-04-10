using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DiagDash
{
    internal static class RequestExtensions
    {
        // taken from System.Web.Mvc
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            return request["X-Requested-With"] == "XMLHttpRequest" || (request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest");
        }
    }
}
