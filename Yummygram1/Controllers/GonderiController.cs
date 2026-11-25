using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yummygram1;

namespace Yummygram.Controllers
{
    public class GonderiController : Controller
    {
        private YummygramDBEntities db = new YummygramDBEntities();

        [HttpGet]
        public ActionResult Olustur()
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            ViewBag.Kategoriler = new SelectList(
                db.Kategoriler.ToList(),
                "KategoriID",
                "KategoriAdi"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Olustur(
            string baslik,
            string tarif,
            int kategoriID,
            HttpPostedFileBase fotograf)
        {
            if (Session["KullaniciID"] == null)
                return RedirectToAction("Giris", "Hesap");

            ViewBag.Kategoriler = new SelectList(
                db.Kategoriler.ToList(),
                "KategoriID",
                "KategoriAdi"
            );

            if (string.IsNullOrWhiteSpace(baslik) ||
                string.IsNullOrWhiteSpace(tarif) ||
                fotograf == null ||
                fotograf.ContentLength == 0)
            {
                ViewBag.Hata = "Bütün alanları doldurmalısın.";
                return View();
            }

            string uzanti = Path.GetExtension(fotograf.FileName).ToLower();
            string[] izinli = { ".jpg", ".jpeg", ".png" };

            if (!izinli.Contains(uzanti) ||
                fotograf.ContentLength > 5 * 1024 * 1024)
            {
                ViewBag.Hata = "JPG veya PNG, en fazla 5 MB fotoğraf yükleyebilirsin.";
                return View();
            }

            var kategori = db.Kategoriler
                .FirstOrDefault(x => x.KategoriID == kategoriID);

            if (kategori == null)
            {
                ViewBag.Hata = "Geçerli bir kategori seçmelisin.";
                return View();
            }

            string klasor = Server.MapPath("~/Uploads/Gonderiler/");

            if (!Directory.Exists(klasor))
                Directory.CreateDirectory(klasor);

            string dosyaAdi = Guid.NewGuid().ToString() + uzanti;
            fotograf.SaveAs(Path.Combine(klasor, dosyaAdi));

            Gonderiler yeni = new Gonderiler();

            yeni.KullaniciID = (int)Session["KullaniciID"];
            yeni.KategoriID = kategoriID;
            yeni.Baslik = baslik;
            yeni.Tarif = tarif;
            yeni.FotografYolu = "/Uploads/Gonderiler/" + dosyaAdi;
            yeni.PaylasimTarihi = DateTime.Now;

            db.Gonderiler.Add(yeni);
            db.SaveChanges();

            return RedirectToAction("Index", "Profil");
        }

        [HttpGet]
        public ActionResult Detay(int id)
        {
            var gonderi = db.Gonderiler
                .FirstOrDefault(x => x.GonderiID == id);

            if (gonderi == null)
                return HttpNotFound();

            var degerlendirmeler = gonderi.Degerlendirmeler.ToList();

            ViewBag.Degerlendirmeler = degerlendirmeler;
            ViewBag.Ortalama = degerlendirmeler.Count > 0
                ? degerlendirmeler.Average(x => x.YildizSayisi)
                : 0;

            ViewBag.DegerlendirmeSayisi = degerlendirmeler.Count;

            return View(gonderi);
        }
    }
}