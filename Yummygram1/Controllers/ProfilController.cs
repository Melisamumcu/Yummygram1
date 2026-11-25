using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yummygram1;

namespace Yummygram.Controllers
{
    public class ProfilController : Controller
    {
        private YummygramDBEntities db = new YummygramDBEntities();

        public ActionResult Index()
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            int id = (int)Session["KullaniciID"];

            var kullanici = db.Kullanicilar
                .FirstOrDefault(x => x.KullaniciID == id);

            if (kullanici == null)
                return RedirectToAction("Giris", "Hesap");

            return View(kullanici);
        }

        [HttpGet]
        public ActionResult Duzenle()
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            int id = (int)Session["KullaniciID"];

            var kullanici = db.Kullanicilar
                .FirstOrDefault(x => x.KullaniciID == id);

            if (kullanici == null)
                return RedirectToAction("Giris", "Hesap");

            return View(kullanici);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duzenle(string biyografi)
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            int id = (int)Session["KullaniciID"];

            var kullanici = db.Kullanicilar
                .FirstOrDefault(x => x.KullaniciID == id);

            if (kullanici == null)
                return RedirectToAction("Giris", "Hesap");

            kullanici.Biyografi = biyografi;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FotografYukle(HttpPostedFileBase fotograf)
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            if (fotograf == null || fotograf.ContentLength == 0)
                return RedirectToAction("Index");

            string uzanti = Path.GetExtension(fotograf.FileName).ToLower();

            string[] izinliUzantilar = { ".jpg", ".jpeg", ".png" };

            if (!izinliUzantilar.Contains(uzanti) ||
                fotograf.ContentLength > 5 * 1024 * 1024)
                return RedirectToAction("Index");

            int id = (int)Session["KullaniciID"];

            var kullanici = db.Kullanicilar
                .FirstOrDefault(x => x.KullaniciID == id);

            if (kullanici == null)
                return RedirectToAction("Giris", "Hesap");

            string klasor = Server.MapPath("~/Uploads/ProfilFotograflari/");

            if (!Directory.Exists(klasor))
                Directory.CreateDirectory(klasor);

            string dosyaAdi = Guid.NewGuid().ToString() + uzanti;
            string yol = Path.Combine(klasor, dosyaAdi);

            fotograf.SaveAs(yol);

            kullanici.ProfilFotografi =
                "/Uploads/ProfilFotograflari/" + dosyaAdi;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Cikis()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Giris", "Hesap");
        }
    }
}