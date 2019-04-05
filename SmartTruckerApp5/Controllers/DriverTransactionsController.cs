using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SmartTruckerApp5.Models;

namespace SmartTruckerApp5.Controllers
{
    public class DriverTransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       // private ApplicationUserManager _userManager;

        // GET: DriverTransactions
        public ActionResult Index()
        {
            var driverTransactionsObj = db.driverTransactionsObj.Include(d => d.transactions);
            return View(driverTransactionsObj.ToList());
        }

        // GET: DriverTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DriverTransactions driverTransactions = db.driverTransactionsObj.Find(id);
            if (driverTransactions == null)
            {
                return HttpNotFound();
            }
            return View(driverTransactions);
        }

        // GET: DriverTransactions/Create
        public ActionResult Create()
        {
            ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey");
            var driver = (from gh in db.userRoles
                          join jk in db.Users
                          on gh.userKey equals jk.Id
                           where gh.roleKey == "Driver"
                          select new { jk.Id, jk.UserName }).ToList();

            
           
           ViewBag.UserDetailsKey = new SelectList(driver, "UserName", "UserName");
            return View();
        }

        // POST: DriverTransactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserDetailsKey,TransactionsKey")] DriverTransactions driverTransactions)
        {
            if (ModelState.IsValid)
            {
                if (driverTransactions.DriverAvailabilityChecker()==false)
                {
                    if (driverTransactions.DriverChecker() == true)
                    {
                        ViewBag.message = "Driver you selected is not available until " + driverTransactions.DriverAvailabilityDate().Date;
                        ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey", driverTransactions.TransactionsKey);
                        ViewBag.UserDetailsKey = new SelectList(db.userDetails, "UserDetailsKey", "Username", driverTransactions.UserDetailsKey);
                        return View(driverTransactions);
                    }
                    else
                    {
                        db.driverTransactionsObj.Add(driverTransactions);
                        db.SaveChanges();
                        ViewBag.message = " The Driver has been assigned";
                        ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey", driverTransactions.TransactionsKey);
                        ViewBag.UserDetailsKey = new SelectList(db.userDetails, "UserDetailsKey", "Username", driverTransactions.UserDetailsKey);
                        return View(driverTransactions);
                    }
                }
                else
                {
                    ViewBag.message = " The Driver you selected is already assigned to this task";
                    ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey", driverTransactions.TransactionsKey);
                    ViewBag.UserDetailsKey = new SelectList(db.userDetails, "UserDetailsKey", "Username", driverTransactions.UserDetailsKey);
                    return View(driverTransactions);
                }
            }

            ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey", driverTransactions.TransactionsKey);
            ViewBag.UserDetailsKey = new SelectList(db.userDetails, "UserDetailsKey", "Username", driverTransactions.UserDetailsKey);
            return View(driverTransactions);
        }

        // GET: DriverTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DriverTransactions driverTransactions = db.driverTransactionsObj.Find(id);
            if (driverTransactions == null)
            {
                return HttpNotFound();
            }
            ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey", driverTransactions.TransactionsKey);
            ViewBag.UserDetailsKey = new SelectList(db.userDetails, "UserDetailsKey", "Username", driverTransactions.UserDetailsKey);
            return View(driverTransactions);
        }

        // POST: DriverTransactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserDetailsKey,TransactionsKey")] DriverTransactions driverTransactions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(driverTransactions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey", driverTransactions.TransactionsKey);
            ViewBag.UserDetailsKey = new SelectList(db.userDetails, "UserDetailsKey", "Username", driverTransactions.UserDetailsKey);
            return View(driverTransactions);
        }

        // GET: DriverTransactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DriverTransactions driverTransactions = db.driverTransactionsObj.Find(id);
            if (driverTransactions == null)
            {
                return HttpNotFound();
            }
            return View(driverTransactions);
        }

        // POST: DriverTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DriverTransactions driverTransactions = db.driverTransactionsObj.Find(id);
            db.driverTransactionsObj.Remove(driverTransactions);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
