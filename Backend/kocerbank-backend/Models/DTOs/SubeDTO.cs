using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class SubeDTO : BaseDTO
    {
        public string SubeAdi { get; set; } = string.Empty;
        public string SubeKodu { get; set; } = string.Empty;
        public string SubeTelefonNo { get; set; } = string.Empty;
        public string SubeAdres { get; set; } = string.Empty;
        public AktifPasifDurumlari SubeDurumKodu { get; set; }
        public DateTime KayitOlusturmaTarihi { get; set; }
    }

    public class SubeAramaKriterleriDTO
    {
        public long? Id { get; set; }
        public string? SubeAdi { get; set; }
        public string? SubeKodu { get; set; }
        public string? SubeTelefonNo { get; set; }
        public string? SubeAdres { get; set; }
        public AktifPasifDurumlari? SubeDurumKodu { get; set; }

        public DateTime? AcilisTarihiBaslangic { get; set; }
        public DateTime? AcilisTarihiBitis { get; set; }
    }

    public class SubeDashboardDTO
    {
        public long ToplamSube { get; set; }
        public long AktifSayi { get; set; }
        public long PasifSayi { get; set; }
        public string? SonSubeAdi { get; set; }
    }
}