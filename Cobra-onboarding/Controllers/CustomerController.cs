using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cobra_onboarding.Models;
using Cobra_onboarding.Models;

namespace Cobra_onboarding.Controllers
{
    public class CustomerController : Controller
    {
        private CobraOnboardingContext db = new CobraOnboardingContext();

        // GET: Customer
        [HttpGet]
        public JsonResult Get()
        {
            var people = from p in db.People
                         select new
                         {
                             Id = p.Id,
                             Name = p.Name,
                             Address1 = p.Address1,
                             Address2 = p.Address2,
                             City = p.City
                         };
            return Json(people.ToList(),JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// POST: Adding new customer
        /// </summary>
        /// <param name="name"></param>
        /// <param name="add1"></param>
        /// <param name="add2"></param>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddCustomer(Person customer/*string name, string add1, string add2, string city*/)
        {
            //if(String.IsNullOrEmpty(name) || String.IsNullOrEmpty(add1) || String.IsNullOrEmpty(city))
            //{
            //    return Json("Error: Data is invalid!", JsonRequestBehavior.DenyGet);
            //}

            if (customer == null)
            {
                return Json("Error: Data is invalid!", JsonRequestBehavior.DenyGet);
            }

            Person p = new Person()
            {
                Name=customer.Name,//name,
                Address1 = customer.Address1,//add1,
                Address2 = customer.Address2,//add2,
                City = customer.City//city
            };

            try
            {
                db.People.Add(p);
                db.SaveChanges();
                int id = db.People.OrderBy(e => e.Id).Max(e=>e.Id);
                return Json(id, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(e.Message,JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// POST: Update a customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult EditCustomer(Person customer/*string id, string name, string add1, string add2, string city*/)
        {
            if (customer==null/*String.IsNullOrEmpty(id) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(add1) || String.IsNullOrEmpty(city)*/)
            {
                return Json("Error: Data is invalid!", JsonRequestBehavior.DenyGet);
            }

            int personId = Convert.ToInt32(customer.Id/*id*/);
            var p = db.People.Where(e => e.Id.Equals(personId)).First();
            if(p == null)
            {
                return Json("Error: Customer is not existed", JsonRequestBehavior.DenyGet);
            }

            p.Name = customer.Name;//name;
            p.Address1 = customer.Address1; //add1;
            p.Address2 = customer.Address2;//add2;
            p.City = customer.City;//city;

            db.Entry(p).State = EntityState.Modified;
            db.SaveChanges();

            return Json(personId, JsonRequestBehavior.DenyGet); 
        }

        /// <summary>
        /// Delete: Delete a customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DeleteCustomer(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return Json("Error: value is invalid", JsonRequestBehavior.DenyGet);
            }

            int personId = Convert.ToInt32(id);
            var ohs = db.OrderHeaders.Where(o => o.PersonId.Equals(personId)).ToList();
            foreach(var oh in ohs)
            {
                var ods = oh.OrderDetails.ToList();
                db.OrderDetails.RemoveRange(ods);
            }
            db.OrderHeaders.RemoveRange(ohs);
            db.SaveChanges();

            var p = db.People.Where(e => e.Id.Equals(personId)).First();
            if (p == null)
            {
                return Json("Error: Customer is not existed", JsonRequestBehavior.DenyGet);
            }

            try
            {
                db.People.Remove(p);
                db.SaveChanges();
            }
            catch(Exception e)
            {
                return Json("Error: Cannot delete", JsonRequestBehavior.DenyGet);
            }

            return Json(personId, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        //// GET: Customer/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Person person = db.People.Find(id);
        //    if (person == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(person);
        //}

        //// GET: Customer/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Customer/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Address1,Address2,City")] Person person)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.People.Add(person);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(person);
        //}

        //// GET: Customer/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Person person = db.People.Find(id);
        //    if (person == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(person);
        //}

        //// POST: Customer/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,Name,Address1,Address2,City")] Person person)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(person).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(person);
        //}

        //// GET: Customer/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Person person = db.People.Find(id);
        //    if (person == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(person);
        //}

        //// POST: Customer/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Person person = db.People.Find(id);
        //    db.People.Remove(person);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
