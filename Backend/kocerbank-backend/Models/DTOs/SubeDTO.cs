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
    }

    public class SubeAramaKriterleriDTO
    {
        public long? Id { get; set; }
        public string? SubeAdi { get; set; }
        public string? SubeKodu { get; set; }
        public string? SubeTelefonNo { get; set; }
        public string? SubeAdres { get; set; }
        public AktifPasifDurumlari? SubeDurumKodu { get; set; }
    }
}