using SoLoud.Models;
using System.Web.Mvc;

namespace SoLoud.Controllers
{
    public class FileController : Controller
    {
        private SoLoudContext context = new SoLoudContext();
        //
        // GET: /File/
        public ActionResult Index(int id)
        {
            var fileToRetrieve = context.Files.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
    }
}