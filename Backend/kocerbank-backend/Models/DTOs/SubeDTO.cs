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
            = AktifPasifDurumlari.None;
    }
}