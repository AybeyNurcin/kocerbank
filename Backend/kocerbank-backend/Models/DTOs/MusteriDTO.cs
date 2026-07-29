using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class MusteriDTO : BaseDTO
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Eposta { get; set; } = string.Empty;
        public DateTime? DogumTarihi { get; set; }
        public string TelefonNo { get; set; } = string.Empty;
        public string? TCKN { get; set; }
        public CinsiyetDurumlari? Cinsiyet { get; set; }
        public string? VKN { get; set; }
        public MusteriTipiDurumlari MusteriTipi { get; set; }
        public string SubeSubeKodu { get; set; } = string.Empty;
        public AktifPasifDurumlari DurumKodu { get; set; }
        public string? Unvan { get; set; }
        public DateTime KayitOlusturmaTarihi { get; set; }
    }
    public class MusteriAramaKriterleriDTO
    {
        public long? Id { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Eposta { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string? TelefonNo { get; set; }
        public string? TCKN { get; set; }
        public CinsiyetDurumlari? Cinsiyet { get; set; }
        public string? VKN { get; set; }
        public MusteriTipiDurumlari? MusteriTipi { get; set; }
        public string? SubeSubeKodu { get; set; }
        public AktifPasifDurumlari? DurumKodu { get; set; }
        public string? Unvan { get; set; }
        public DateTime? KayitOlusturmaTarihi { get; set; }
    }
}

