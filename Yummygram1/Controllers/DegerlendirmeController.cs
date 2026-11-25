using System;
using System.Linq;
using System.Web.Mvc;
using Yummygram1;

namespace Yummygram.Controllers
{
    public class DegerlendirmeController : Controller
    {
        private YummygramDBEntities db = new YummygramDBEntities();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Ekle(int gonderiID, int yildizSayisi)
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            if (yildizSayisi < 1 || yildizSayisi > 5)
                return RedirectToAction("Index", "Profil");

            int kullaniciID = (int)Session["KullaniciID"];

            var gonderi = db.Gonderiler
                .FirstOrDefault(x => x.GonderiID == gonderiID);

            if (gonderi == null)
                return RedirectToAction("Index", "Profil");

            var mevcut = db.Degerlendirmeler.FirstOrDefault(x =>
                x.GonderiID == gonderiID &&
                x.KullaniciID == kullaniciID);

            if (mevcut == null)
            {
                Degerlendirmeler yeni = new Degerlendirmeler();

                yeni.GonderiID = gonderiID;
                yeni.KullaniciID = kullaniciID;
                yeni.YildizSayisi = yildizSayisi;
                yeni.DegerlendirmeTarihi = DateTime.Now;

                db.Degerlendirmeler.Add(yeni);
            }
            else
            {
                mevcut.YildizSayisi = yildizSayisi;
                mevcut.DegerlendirmeTarihi = DateTime.Now;
            }

            db.SaveChanges();

            return RedirectToAction("Detay", "Gonderi",
                new { id = gonderiID });
        }
    }
}