using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LF.Toolkit.Web.Security
{
    public interface IResultHandler
    {
        void Encrypt(string encryptionKey, JsonResult plainResult);
    }
}