using System;
using System.Linq;
using System.Web.Mvc;
using Yummygram1;

namespace Yummygram.Controllers
{
    public class YorumController : Controller
    {
        private YummygramDBEntities db = new YummygramDBEntities();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ekle(int gonderiID, string yorumMetni)
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            if (string.IsNullOrWhiteSpace(yorumMetni))
                return RedirectToAction("Detay", "Gonderi",
                    new { id = gonderiID });

            var gonderi = db.Gonderiler
                .FirstOrDefault(x => x.GonderiID == gonderiID);

            if (gonderi == null)
                return HttpNotFound();

            Yorumlar yeniYorum = new Yorumlar();

            yeniYorum.GonderiID = gonderiID;
            yeniYorum.KullaniciID = (int)Session["KullaniciID"];
            yeniYorum.YorumMetni = yorumMetni;
            yeniYorum.YorumTarihi = DateTime.Now;

            db.Yorumlar.Add(yeniYorum);
            db.SaveChanges();

            return RedirectToAction("Detay", "Gonderi",
                new { id = gonderiID });
        }
    }
}