using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yoklamaVsty
{
    public class TalebelerModel : DbContext
    {
        public DbSet<Ogrenci> Talebeler { get; set; }
        public DbSet<Hoca> Hocalar { get; set; }
        public DbSet<YoklamaModel> YoklamaTable { get; set; }

        public DbSet<geciciTarih> geciciTarihler { get; set; }
        public DbSet<BeniHatirla> BeniHatirlaTable { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DBPath.DatabasePath}");
        }
    }
    public class Ogrenci
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; }

        [Required]
        public string Soyad { get; set; }
    }
    public class geciciTarih
    {
        public int Id { get; set; }
        public DateTime Tarih { get; set; }
        public string Ogun { get; set; } // "Sabah" veya "Akşam"
    }


    public class Hoca
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Ad { get; set; }

        [Required]
        public string Soyad { get; set; }

        [Required]
        public string KullaniciAdi { get; set; }

        [Required]
        public string Sifre { get; set; }
        public string SifreKurtarmaSorusu { get; set; }
        public string Cevap { get; set; }
    }

    public class YoklamaModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime Tarih { get; set; }

        [Required]
        public int TalebeId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }

        public string zaman { get; set; }

        [Required]
        public string Durum { get; set; }
    }

    public class BeniHatirla
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public int Durum { get; set; }
    }

}
