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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactions = db.transactions.Include(t => t.trucks);
            return View(transactions.ToList());
        }
        [ChildActionOnly]
        public ActionResult CreateTransa()
        {
            ViewBag.RegNo = new SelectList(db.trucks, "RegistrationNo", "RegistrationNo");

            return PartialView("_CreateTransactionPlanner");
        }



        [ChildActionOnly]
        public ActionResult assignDriver()
        {
            ViewBag.UserKey = new SelectList(db.userDetails, "UserKey", "UserName");
            ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey");
            return PartialView("_assignDriver");
        }
        [HttpPost]
        public ActionResult assignDriver(DriverTransactions driver)
        {
            int counter = 0;
            int counter2 = 0;
            var frmdb = (from gh in db.driverTransactionsObj
                         where gh.TransactionsKey == driver.TransactionsKey
                         select gh.TransactionsKey).ToList();
            var frmdb2 = (from gh in db.driverTransactionsObj
                          where gh.TransactionsKey == driver.TransactionsKey && gh.UserDetailsKey == driver.UserDetailsKey
                          select gh.TransactionsKey).ToList();

            foreach (var itm in frmdb)
            { counter++; }
            foreach (var itm in frmdb2)
            { counter2++; }
            if (counter2 < 1)
            {
                if (counter > 2)
                { ViewBag.message = "The Job you selected already has two drivers"; }
                else if (ModelState.IsValid)
                {
                    db.driverTransactionsObj.Add(driver);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else
            {

            }

            ViewBag.UserKey = new SelectList(db.userDetails, "UserKey", "UserName");
            ViewBag.TransactionsKey = new SelectList(db.transactions, "TransactionsKey", "TransactionsKey");
            return PartialView("_assignDriver");
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transactions transactions = db.transactions.Find(id);
            if (transactions == null)
            {
                return HttpNotFound();
            }
            return View(transactions);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.TrucksID = new SelectList(db.trucks, "TrucksKey", "RegistrationNo");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionsKey,EstimatedDelivery,Earnings,StartDateTime,FuelConsumed,TrucksID")] Transactions transactions)
        {
            if (ModelState.IsValid)
            {
                if (transactions.TruckAvailabilityChecker() == true)
                {
                    ViewBag.TrucksID = new SelectList(db.trucks, "TrucksKey", "RegistrationNo", transactions.TrucksID);
                    ViewBag.message = "Truck you selected is not available until "+transactions.TruckAvailabilidate();
                    return View();
                }
                else
                {
                    if (transactions.StartDateTime <= transactions.EstimatedDelivery)
                    {
                        db.transactions.Add(transactions);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.message = "Estimated come back date cannot  be before the pick up date ";
                        ViewBag.TrucksID = new SelectList(db.trucks, "TrucksKey", "RegistrationNo", transactions.TrucksID);
                        return View();
                    }

                }
            }
            ViewBag.TrucksID = new SelectList(db.trucks, "TrucksKey", "RegistrationNo", transactions.TrucksID);
            return View(transactions);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transactions transactions = db.transactions.Find(id);
            if (transactions == null)
            {
                return HttpNotFound();
            }
            ViewBag.TrucksID = new SelectList(db.trucks, "TrucksKey", "RegistrationNo", transactions.TrucksID);
            return View(transactions);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionsKey,EstimatedDelivery,Earnings,StartDateTime,FuelConsumed,TrucksID")] Transactions transactions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transactions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TrucksID = new SelectList(db.trucks, "TrucksKey", "RegistrationNo", transactions.TrucksID);
            return View(transactions);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transactions transactions = db.transactions.Find(id);
            if (transactions == null)
            {
                return HttpNotFound();
            }
            return View(transactions);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transactions transactions = db.transactions.Find(id);
            db.transactions.Remove(transactions);
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
