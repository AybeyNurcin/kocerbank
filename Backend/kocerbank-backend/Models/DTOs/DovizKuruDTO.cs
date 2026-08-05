namespace kocerbank_backend.Models.DTOs
{
    public class DovizKuruDosyasiDTO
    {
        public DateTime KurTarihi { get; set; }

        public Dictionary<string, DovizKuruDTO> Kurlar
        {
            get;
            set;
        } = new();
    }


    public class DovizKuruDTO
    {
        public decimal Alis { get; set; }

        public decimal Satis { get; set; }
    }
}