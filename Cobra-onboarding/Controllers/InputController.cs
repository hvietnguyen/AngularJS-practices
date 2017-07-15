using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cobra_onboarding.Controllers
{
    public class InputController : Controller
    {
        // GET: InputText
        public ActionResult Text()
        {
            return PartialView();
        }
    }
}