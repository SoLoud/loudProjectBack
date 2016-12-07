using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SoLoud.Models;
using SoLoud.Controllers;
using System.Drawing;

namespace SoLoud.Controllers
{
    [Authorize]
    public class ContestsController : BaseController
    {
        private SoLoudContext db = new SoLoudContext();

        // GET: Contests
        public ActionResult Index()
        {
            var contests = db.Contests/*.Include(c => c.User)*/;
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
            return View(new ContestSentItem() { EndingAt = endDate });
        }

        public class ContestSentItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTimeOffset EndingAt { get; set; }
            public Categoies Category { get; set; }
            public string HashTags { get; set; }
            public HttpPostedFileBase ProductImage { get; set; }
        }

        private string getImage()
        {
            var bitmapImage = new Bitmap(500, 300);
            var g = Graphics.FromImage(bitmapImage);

            string filepath = Server.MapPath("~/Images/" + Guid.NewGuid().ToString() + ".jpg");

            bitmapImage.Save(filepath, System.Drawing.Imaging.ImageFormat.Jpeg);

            return filepath;
        }

        // POST: Contests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContestSentItem contest)
        {
            //if (!Request.Content.IsMimeMultipartContent())
            //{
            //    this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            //}


            //Stream req = Request.RequestContext.Content.ReadAsStreamAsync().Result;
            //HttpMultipartParser parser = new HttpMultipartParser(req, "file");

            //if (parser.Success)
            //{
            //    MemoryStream ms = new MemoryStream(parser.FileContents);
            //    HSSFWorkbook templateWorkbook = new HSSFWorkbook(ms);

            //    HSSFSheet sheet = (HSSFSheet)templateWorkbook.GetSheetAt(0);

            //}

            //var asd = new byte[];
            //Request.InputStream.Read()

            var a = Request;
            var newContest = new Contest();
            newContest.Id = Guid.NewGuid().ToString();
            newContest.UserId = UserId;
            newContest.Title = contest.Title;
            newContest.Description = contest.Description;
            newContest.EndingAt = contest.EndingAt;
            newContest.Category = contest.Category;

            //var imageurl = getImage();
            //newContest.ProductImageUrl = imageurl;

            if (contest.ProductImage != null && contest.ProductImage.ContentLength > 0)
            {
                var newPhoto = new File()
                {
                    FileName = System.IO.Path.GetFileName(contest.ProductImage.FileName),
                    FileType = FileType.Photo,
                    ContentType = contest.ProductImage.ContentType
                };
                using (var reader = new System.IO.BinaryReader(contest.ProductImage.InputStream))
                {
                    newPhoto.Content = reader.ReadBytes(contest.ProductImage.ContentLength);
                }
                newContest.Photos = new List<File>() {
                    newPhoto
                };
            }

            var HashTags = new List<HashTag>();
            foreach (var Tag in contest.HashTags.Split(','))
            {
                HashTags.Add(new HashTag()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = Tag.Trim(),
                    IsRequired = true,
                    ItemId = newContest.Id
                });
            }

            if (ModelState.IsValid)
            {
                db.HashTags.AddRange(HashTags);
                db.Contests.Add(newContest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "Hometown", newContest.UserId);
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
    [Authorize]
    [RoutePrefix("api/Contests")]
    public class ContestsApiController : BaseApiController
    {
 
        [HttpGet]
        public List<Contest> Get()
        {
            var context = new SoLoudContext();
            
            return context.Contests.Include("HashTags").ToList();
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
