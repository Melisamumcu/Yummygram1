using System.ComponentModel.DataAnnotations;

namespace Yummygram.Models
{
    public class HesapViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string Eposta { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Sifre { get; set; }

        public string KullaniciAdi { get; set; }

        [DataType(DataType.Password)]
        public string SifreTekrar { get; set; }
    }
}