using System;
using System.Linq;
using System.Web.Mvc;
using Yummygram1;

namespace Yummygram.Controllers
{
    public class AnaSayfaController : Controller
    {
        private YummygramDBEntities db = new YummygramDBEntities();

        public ActionResult Index()
        {
            var gonderiler = db.Gonderiler
                .OrderByDescending(x => x.PaylasimTarihi)
                .ToList();

            ViewBag.OrtalamaPuanlar = gonderiler.ToDictionary(
                x => x.GonderiID,
                x => x.Degerlendirmeler.Any()
                    ? x.Degerlendirmeler.Average(d => (double)d.YildizSayisi)
                    : 0
            );

            ViewBag.YorumSayilari = gonderiler.ToDictionary(
                x => x.GonderiID,
                x => x.Yorumlar.Count()
            );

            return View(gonderiler);
        }

        public ActionResult Hakkinda()
        {
            return View();
        }
    }
}