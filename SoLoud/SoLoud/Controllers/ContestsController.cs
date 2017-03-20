using SoLoud.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using System.Web.Mvc;

namespace SoLoud.Controllers
{
    [System.Web.Mvc.Authorize]
    [System.Web.Mvc.RoutePrefix("Contests")]
    public class ContestsController : BaseController
    {
        private SoLoudContext db = new SoLoudContext();

        // GET: Contests
        public ActionResult Index()
        {
            var contests = db.Contests.Include(c => c.ExamplePhotos).Include(c => c.ProductPhotos);
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
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.Route("Create")]
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
        [System.Web.Mvc.Route("Create")]
        [System.Web.Mvc.HttpPost]
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
                newContest.ExamplePhotos = new List<File>() {
                    newPhoto
                };
            }

            if (ModelState.IsValid)
            {
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
        [System.Web.Http.HttpPost]
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
        [System.Web.Http.HttpPost, System.Web.Http.ActionName("Delete")]
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
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Contests")]
    public class ContestsApiController : BaseApiController
    {
        private SoLoudContext db = new SoLoudContext();

        public ContestsApiController()
        {
            //this.Configuration.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            //this.Configuration.Formatters.JsonFormatter.SerializerSettings.
        }

        // GET: api/Contests2
        [EnableQuery]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("")]
        public IQueryable<Contest> GetContentItems()
        {
            return db.Contests;
        }

        // GET: api/Contests2/5
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("{id}")]
        public IHttpActionResult GetContest(string id)
        {
            Contest contest = db.Contests.Find(id);
            if (contest == null)
            {
                return NotFound();
            }

            return Ok(contest);
        }

        // PUT: api/Contests2/5
        [ResponseType(typeof(void))]
        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("{id}")]
        public IHttpActionResult PutContest(string id, Contest contest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contest.Id)
            {
                return BadRequest();
            }

            db.Entry(contest).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Contests2
        [ResponseType(typeof(Contest))]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("")]
        public IHttpActionResult PostContest(Contest contest)
        {
            ModelState.Clear();
            contest.UserId = UserId;
            if (contest.Id == null)
                contest.Id = Guid.NewGuid().ToString();
            Validate(contest);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Contests.Add(contest);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ContestExists(contest.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            catch(Exception e)
            {

            }

            return CreatedAtRoute("DefaultApi", new { id = contest.Id }, contest);
        }

        // DELETE: api/Contests2/5
        [ResponseType(typeof(Contest))]
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("{id}")]
        public IHttpActionResult DeleteContest(string id)
        {
            Contest contest = db.Contests.Find(id);
            if (contest == null)
            {
                return NotFound();
            }

            db.Contests.Remove(contest);
            db.SaveChanges();

            return Ok(contest);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ContestExists(string id)
        {
            return db.Contests.Count(e => e.Id == id) > 0;
        }
    }
}
