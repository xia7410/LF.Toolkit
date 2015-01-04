using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LF.Toolkit.Web.Exceptions
{
    public class InternalServerException : Exception
    {
        public InternalServerException(string message)
            : base(message)
        {

        }
    }
}