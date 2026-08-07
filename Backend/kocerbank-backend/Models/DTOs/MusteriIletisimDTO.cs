namespace kocerbank_backend.Models.DTOs
{
    public class MusteriIletisimDTO : BaseDTO
    {
        public string TelefonNo { get; set; } =
            string.Empty;

        public string? EvTelefonNo { get; set; }

        public string? IsTelefonNo { get; set; }

        public string? EvAdres { get; set; }

        public string? IsAdres { get; set; }

        public string Eposta { get; set; } =
            string.Empty;

        public long MusteriBilgileriId { get; set; }
    }

    public class MusteriIletisimAramaKriterleriDTO
    {
        public string? TelefonNo { get; set; }

        public string? EvTelefonNo { get; set; }

        public string? IsTelefonNo { get; set; }

        public string? EvAdres { get; set; }

        public string? IsAdres { get; set; }

        public string? Eposta { get; set; }

        public long MusteriBilgileriId { get; set; }

        // Frontend tarafından doldurulmaz.
        // MusteriIletisimService giriş yapan
        // personelin sicilini buraya yazar.
        public string? RecordUser { get; set; }
    }
}