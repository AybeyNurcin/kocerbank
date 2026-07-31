using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class ParaTransferiDTO : BaseDTO
    {
        public string GonderenIBAN { get; set; } = string.Empty;
        public string AliciIBAN { get; set; } = string.Empty;
        public TransferTipleri TransferTipi { get; set; }
        public decimal Miktar { get; set; }
        public string Aciklama { get; set; } = string.Empty;
        public DovizTipleri GonderenDovizTipi { get; set; }
        public DovizTipleri AliciDovizTipi { get; set; }
        public DateTime TarihSaat { get; set; }
    }

    public class ParaTransferiAramaKriterleriDTO
    {
        public long? Id { get; set; }
        public string? GonderenIBAN { get; set; }
        public string? AliciIBAN { get; set; }
        public TransferTipleri? TransferTipi { get; set; }
        public decimal? Miktar { get; set; }
        public string? Aciklama { get; set; }
        public DovizTipleri? GonderenDovizTipi { get; set; }
        public DovizTipleri? AliciDovizTipi { get; set; }
        public DateTime? TarihSaat { get; set; }
    }
}

