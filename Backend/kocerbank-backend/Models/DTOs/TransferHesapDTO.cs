namespace kocerbank_backend.Models.DTOs
{
    public class TransferHesapDTO : HesapDTO
    {
        public string HesapSahibi { get; set; } =
            string.Empty;
    }
}