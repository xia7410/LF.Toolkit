using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace LF.Toolkit.Web.Exceptions
{
    public class BadRequestException : Exception
    {
        public HttpStatusCode StatusCode { get { return HttpStatusCode.BadRequest; } }

        public string SubStatusCode { get; private set; }

        public string StatusDescription { get; private set; }

        public BadRequestException(string subStatusCode)
            : this(subStatusCode, null)
        {

        }

        public BadRequestException(string subStatusCode, string statusDescription)
        {
            SubStatusCode = subStatusCode;
            StatusDescription = statusDescription;
        }
    }
}