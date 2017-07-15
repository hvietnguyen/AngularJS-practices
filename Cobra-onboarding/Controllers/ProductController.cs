using Cobra_onboarding.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cobra_onboarding.Controllers
{
    public class ProductController : Controller
    {
        private CobraOnboardingContext db = new CobraOnboardingContext();
        /// <summary>
        /// GET: get all products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult Get()
        {
            var products = from p in db.Products
                           select new
                           {
                               Id = p.Id,
                               Price = p.Price,
                               ProductName = p.ProductName
                           };
            return Json(products.ToList(),JsonRequestBehavior.AllowGet);
        }
    }
}