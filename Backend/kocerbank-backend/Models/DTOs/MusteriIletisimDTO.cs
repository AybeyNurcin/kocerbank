namespace kocerbank_backend.Models.DTOs
{
    public class MusteriIletisimDTO : BaseDTO
    {
        public string TelefonNo { get; set; } = string.Empty;
        public string? EvTelefonNo { get; set; }
        public string? IsTelefonNo { get; set; }
        public string? EvAdres { get; set; }
        public string? IsAdres { get; set; }
        public string Eposta { get; set; } = string.Empty;
        public long MusteriBilgileriId { get; set; }
    }
}