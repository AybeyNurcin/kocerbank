using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class HesapCekYatirDTO
    {
        public long HesapId { get; set; }
        public HesapHareketTipleri IslemTipi { get; set; }
        public decimal Tutar { get; set; }
        public string? RecordUser { get; set; }
        public long HareketId { get; set; }
        public decimal YeniBakiye { get; set; }
    }
}