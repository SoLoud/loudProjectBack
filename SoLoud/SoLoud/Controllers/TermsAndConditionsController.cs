using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SoLoud.Controllers
{
    public class TermsAndConditionsController : Controller
    {
        // GET: TermsAndConditions
        public ActionResult TermsAndConditions()
        {
            return View();
        }

        // GET: PrivacyPolicy
        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}