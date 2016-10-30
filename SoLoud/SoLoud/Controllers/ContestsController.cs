using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoLoud.Models;
using SoLoud.Controllers;

namespace SoLoud.Controllers
{
    public class ContestsController : BaseController
    {
        private SoLoudContext db = new SoLoudContext();

        // GET: Contests
        public ActionResult Index()
        {
            var contests = db.Contests.Include(c => c.User);
            return View(contests.ToList());
        }

        // GET: Contests/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // GET: Contests/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Hometown");
            var endDate = DateTimeOffset.Now.AddDays(14);
            return View(new Contest() { EndingAt = endDate });
        }

        // POST: Contests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Description,CreatedAt,EndingAt,Category,HashTags,Title")] Contest contest)
        {
            contest.Id = Guid.NewGuid().ToString();
            contest.UserId = UserId;

            if (ModelState.IsValid)
            {
                db.HashTags.AddRange(contest.HashTags);
                db.Contests.Add(contest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Hometown", contest.UserId);
            return View(contest);
        }

        // GET: Contests/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Hometown", contest.UserId);
            return View(contest);
        }

        // POST: Contests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Description,CreatedAt,EndingAt,Category")] Contest contest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Hometown", contest.UserId);
            return View(contest);
        }

        // GET: Contests/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contest contest = db.Contests.Find(id);
            if (contest == null)
            {
                return HttpNotFound();
            }
            return View(contest);
        }

        // POST: Contests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Contest contest = db.Contests.Find(id);
            db.Contests.Remove(contest);
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
namespace SoLoud.ApiControllers
{
    [RoutePrefix("api/Contests")]
    public class ContestsController : BaseApiController
    {
        [HttpGet]
        public List<Contest> Get()
        {
            var context = new SoLoudContext();

            return context.Contests.ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public Contest Get(string id)
        {
            var context = new SoLoudContext();

            return context.Contests.FirstOrDefault(x => x.Id == id);
        }
    }
}
