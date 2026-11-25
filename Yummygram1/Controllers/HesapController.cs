using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Mvc;
using Yummygram.Models;
using Yummygram1;

namespace Yummygram.Controllers
{
    public class HesapController : Controller
    {
        YummygramDBEntities db = new YummygramDBEntities();

        // Şifreyi güvenli şekilde hashler
        private string SifreHashle(string sifre)
        {
            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                sifre, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);

                return Convert.ToBase64String(salt) + ":" +
                       Convert.ToBase64String(hash);
            }
        }

        // Girilen şifreyi kontrol eder
        private bool SifreDogrula(string sifre, string kayitliHash)
        {
            if (string.IsNullOrEmpty(kayitliHash))
                return false;

            string[] parcalar = kayitliHash.Split(':');

            if (parcalar.Length != 2)
                return false;

            byte[] salt = Convert.FromBase64String(parcalar[0]);
            byte[] beklenenHash = Convert.FromBase64String(parcalar[1]);

            using (var pbkdf2 = new Rfc2898DeriveBytes(
                sifre, salt, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hesaplananHash = pbkdf2.GetBytes(32);

                if (hesaplananHash.Length != beklenenHash.Length)
                    return false;

                int fark = 0;

                for (int i = 0; i < hesaplananHash.Length; i++)
                    fark |= hesaplananHash[i] ^ beklenenHash[i];

                return fark == 0;
            }
        }

        // Giriş sayfası
        [HttpGet]
        public ActionResult Giris()
        {
            return View();
        }

        // Giriş işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Giris(HesapViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var kullanici = db.Kullanicilar
                .FirstOrDefault(x => x.Eposta == model.Eposta);

            if (kullanici == null ||
                !SifreDogrula(model.Sifre, kullanici.SifreHash))
            {
                ModelState.AddModelError("", "E-posta veya şifre hatalı.");
                return View(model);
            }

            Session["KullaniciID"] = kullanici.KullaniciID;
            Session["KullaniciAdi"] = kullanici.KullaniciAdi;

            return RedirectToAction("Index", "Profil");
        }

        // Kayıt sayfası
        [HttpGet]
        public ActionResult Kayit()
        {
            return View();
        }

        // Kayıt işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Kayit(HesapViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (db.Kullanicilar.Any(x => x.Eposta == model.Eposta))
            {
                ModelState.AddModelError("Eposta", "Bu e-posta zaten kayıtlı.");
                return View(model);
            }

            if (db.Kullanicilar.Any(x => x.KullaniciAdi == model.KullaniciAdi))
            {
                ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            Kullanicilar yeniKullanici = new Kullanicilar();

            yeniKullanici.KullaniciAdi = model.KullaniciAdi;
            yeniKullanici.Eposta = model.Eposta;
            yeniKullanici.SifreHash = SifreHashle(model.Sifre);
            yeniKullanici.KayitTarihi = DateTime.Now;

            db.Kullanicilar.Add(yeniKullanici);
            db.SaveChanges();

            return RedirectToAction("Giris");
        }
    }
}