using LF.Toolkit.Data;
using LF.Toolkit.WebTest.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LF.Toolkit.WebTest.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            int i = StorageProvider<TestStorage>.Factory.Get();
            return Content("select " + i);
        }

    }
}
