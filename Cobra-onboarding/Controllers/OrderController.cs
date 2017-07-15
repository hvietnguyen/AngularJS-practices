using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cobra_onboarding.Models;
using System.Text.RegularExpressions;


namespace Cobra_onboarding.Controllers
{
    public class OrderController : Controller
    {
        private CobraOnboardingContext db = new CobraOnboardingContext();
        /// <summary>
        /// Get all order header join with people, join with order details and join with products 
        /// </summary>
        /// <returns>Json object format</returns>
        [HttpGet]
        public JsonResult Get()
        {
            var query = from oh in db.OrderHeaders
                        join c in db.People on oh.PersonId equals c.Id
                        join od in db.OrderDetails on oh.OrderId equals od.OrderId
                        join p in db.Products on od.ProductId equals p.Id
                        select new
                        {
                            OrderId = oh.OrderId,
                            Date = oh.OrderDate,//.Day + "-" + oh.OrderDate.Month + "-" + oh.OrderDate.Year,
                            CustomerId = c.Id,
                            Name = c.Name,
                            ProductId = p.Id,
                            ProductName = p.ProductName,
                            Price = p.Price
                        };

            var orders = query.ToList().Select(x => new
            {
                OrderId = x.OrderId,
                Date = x.Date.ToString("MM/dd/yyyy"),
                CustomerId = x.CustomerId,
                Name = x.Name,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Price = x.Price
            });
            return Json(orders.ToList(),JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// POST: Add and save new Order
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="date"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddNewOrder(string customerId, DateTime date, string productId)
        {
            if(String.IsNullOrEmpty(customerId) || date == null || String.IsNullOrEmpty(productId))
            {
                return Json("Error: Invalid parameters", JsonRequestBehavior.DenyGet);
            }

            int cid = Convert.ToInt32(customerId);
            int pid = Convert.ToInt32(productId);
            //string[] subStr = Regex.Split(date, "-"); // date format will be dd-MM-yyyy

            try
            {
                // Insert new order header first
                OrderHeader oh = new OrderHeader()
                {
                    PersonId = Convert.ToInt32(customerId),
                    OrderDate = new DateTime(date.Year, date.Month, date.Day)
                };

                db.OrderHeaders.Add(oh);
                db.SaveChanges();

                // Insert new order details with new order header id
                int orderId = db.OrderHeaders.OrderBy(x => x.OrderId).Max(x => x.OrderId);
                OrderDetail od = new OrderDetail()
                {
                    OrderId = orderId,
                    ProductId = Convert.ToInt32(productId)
                };

                db.OrderDetails.Add(od);
                db.SaveChanges();

                // Bundle an order view object and return
                var product = db.Products.Where(x => x.Id.Equals(pid)).First();

                return Json(orderId, JsonRequestBehavior.DenyGet);                
            }
            catch (FormatException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch(IndexOutOfRangeException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch(NullReferenceException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch(Exception e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// POST: Save an edited order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="customerId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveEditedOrder(string orderId, DateTime date, string customerId, string productId)
        {
            if (String.IsNullOrEmpty(orderId) || date == null || String.IsNullOrEmpty(customerId) || String.IsNullOrEmpty(productId))
            {
                return Json("Error: Invalid parameters", JsonRequestBehavior.DenyGet);
            }
            try
            {
                int oid = Convert.ToInt32(orderId);

                OrderHeader oh = db.OrderHeaders.Where(o => o.OrderId.Equals(oid)).First();
                oh.PersonId = Convert.ToInt32(customerId);
                oh.OrderDate = new DateTime(date.Year, date.Month, date.Day);
                db.Entry(oh).State = EntityState.Modified;
                db.SaveChanges();

                var od = db.OrderDetails.Where(o => o.OrderId.Equals(oid)).First();
                od.ProductId = Convert.ToInt32(productId);
                db.Entry(od).State = EntityState.Modified;
                db.SaveChanges();

                return Json(oid, JsonRequestBehavior.DenyGet);
            }
            catch (FormatException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch (IndexOutOfRangeException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch (NullReferenceException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteOrder(string orderId)
        {
            if (String.IsNullOrEmpty(orderId))
            {
                return Json("Error: Invalid parameters", JsonRequestBehavior.DenyGet);
            }
            try
            {
                int oid = Convert.ToInt32(orderId);
                OrderHeader oh = db.OrderHeaders.Where(o => o.OrderId.Equals(oid)).First();
                var odList = db.OrderDetails.Where(o => o.OrderId.Equals(oid)).ToList();
                // remove order details
                db.OrderDetails.RemoveRange(odList);
                db.SaveChanges();
                // remove order header
                db.OrderHeaders.Remove(oh);
                db.SaveChanges();

                return Json(oid, JsonRequestBehavior.DenyGet);

            }
            catch (FormatException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch (IndexOutOfRangeException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch (NullReferenceException e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json($"Error: {e.Message}", JsonRequestBehavior.DenyGet);
            }
        }

        // GET: Order
        public ActionResult Index()
        {
            //var orderHeaders = db.OrderHeaders.Include(o => o.Person);
            //return View(orderHeaders.ToList());
            return View();
        }

        //// GET: Order/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OrderHeader orderHeader = db.OrderHeaders.Find(id);
        //    if (orderHeader == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(orderHeader);
        //}

        //// GET: Order/Create
        //public ActionResult Create()
        //{
        //    ViewBag.PersonId = new SelectList(db.People, "Id", "Name");
        //    return View();
        //}

        //// POST: Order/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "OrderId,OrderDate,PersonId")] OrderHeader orderHeader)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.OrderHeaders.Add(orderHeader);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.PersonId = new SelectList(db.People, "Id", "Name", orderHeader.PersonId);
        //    return View(orderHeader);
        //}

        //// GET: Order/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OrderHeader orderHeader = db.OrderHeaders.Find(id);
        //    if (orderHeader == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.PersonId = new SelectList(db.People, "Id", "Name", orderHeader.PersonId);
        //    return View(orderHeader);
        //}

        //// POST: Order/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "OrderId,OrderDate,PersonId")] OrderHeader orderHeader)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(orderHeader).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.PersonId = new SelectList(db.People, "Id", "Name", orderHeader.PersonId);
        //    return View(orderHeader);
        //}

        //// GET: Order/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    OrderHeader orderHeader = db.OrderHeaders.Find(id);
        //    if (orderHeader == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(orderHeader);
        //}

        //// POST: Order/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    OrderHeader orderHeader = db.OrderHeaders.Find(id);
        //    db.OrderHeaders.Remove(orderHeader);
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
