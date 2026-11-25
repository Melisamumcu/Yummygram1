using System.Linq;
using System.Web.Mvc;
using Yummygram1;

namespace Yummygram.Controllers
{
    public class KesfetController : Controller
    {
        private YummygramDBEntities db = new YummygramDBEntities();

        public ActionResult Index()
        {
            var gonderiler = db.Gonderiler
                .OrderByDescending(x => x.PaylasimTarihi)
                .ToList();

            return View(gonderiler);
        }
    }
}