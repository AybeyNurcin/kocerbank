using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class HesapHareketiDTO : BaseDTO
    {
        public long HesapBilgileriId { get; set; }
        public long? ParaTransferiId { get; set; }
        public HesapHareketTipleri HesapHareketiTipi { get; set; }
        public int Tutar { get; set; }
        public DovizCinsiDurumlari DovizCinsi { get; set; }
        public int OncekiBakiye { get; set; }
        public int SonrakiBakiye { get; set; }
        public DateTime IslemTarihi { get; set; }

        /*
         * Yalnızca giden transfer hareketlerinde
         * (TransferTipi Eft/SwiftEft ya da SWIFT
         * kanalından yapılan Havale/Virman) dolu
         * gelir; diğerlerinde 0'dır. Bilgilendirme
         * amaçlıdır, veritabanında tutulmaz.
         */
        public decimal KomisyonTutari { get; set; }
    }
}