using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class PersonelDTO : BaseDTO
    {
        public string Ad { get; set; } = string.Empty;
        public string Soyad { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public string Sicil { get; set; } = string.Empty;
        public string Sifre { get; set; } = string.Empty;
        public string TCKN { get; set; } = string.Empty;
        public string Adres { get; set; } = string.Empty;
        public string SubeKodu { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TelefonNo { get; set; } = string.Empty;
        public AktifPasifDurumlari DurumKodu { get; set; }
    }
    public class PersonelAramaKriterleriDTO
    {
        public long? Id { get; set; }
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public string? Rol { get; set; }
        public string? Sicil { get; set; }
        public string? TCKN { get; set; }
        public string? Adres { get; set; }
        public string? SubeKodu { get; set; }
        public string? Email { get; set; }
        public string? TelefonNo { get; set; }
        public AktifPasifDurumlari? DurumKodu { get; set; }
    }

    public class PersonelDashboardDTO
    {
        public long ToplamPersonel { get; set; }
        public long AktifSayi { get; set; }
        public long PasifSayi { get; set; }
    }
}