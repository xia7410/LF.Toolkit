using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace LF.Toolkit.Web.Exceptions
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public string StatusDescription { get; private set; }

        public HttpStatusCodeException(HttpStatusCode statusCode)
            : this(statusCode, null)
        {

        }

        public HttpStatusCodeException(HttpStatusCode statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }
    }
}