using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class MusteriDTO : BaseDTO
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public DateTime DogumTarihi { get; set; }
        public string TelefonNo { get; set; } = string.Empty;
        public int TCKN { get; set; }
        public CinsiyetDurumlari Cinsiyet { get; set; }
        public int VKN { get; set; }
        public MusteriTipiDurumlari MusteriTipi { get; set; }
        public string SubeSubeKodu { get; set; } = string.Empty;
        public AktifPasifDurumlari DurumKodu { get; set; }
        public string Unvan { get; set; } = string.Empty;
        public DateTime KayitOlusturmaTarihi { get; set; }
    }
}

