using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class ParaTransferDTO
    {
        /*
         * Frontend tarafından gönderilecek bilgiler
         */

        public string GonderenIBAN { get; set; } = string.Empty;

        public string AliciIBAN { get; set; } = string.Empty;

        public TransferTipleri TransferTipi { get; set; }

        public decimal GonderenTutar { get; set; }

        public string? Aciklama { get; set; }

        public string? RecordUser { get; set; }


        /*
         * Service katmanının IBAN'lardan bulacağı bilgiler
         */

        public long GonderenHesapId { get; set; }

        public long AliciHesapId { get; set; }

        public DovizCinsiDurumlari GonderenDovizTipi { get; set; }

        public DovizCinsiDurumlari AliciDovizTipi { get; set; }


        /*
         * Kur servisi tarafından hesaplanacak bilgi
         */

        public decimal DovizKuru { get; set; }


        /*
         * Prosedürden dönecek sonuç bilgileri
         */

        public long TransferId { get; set; }

        public long GonderenHareketId { get; set; }

        public long AliciHareketId { get; set; }

        public decimal GonderenYeniBakiye { get; set; }

        public decimal AliciYeniBakiye { get; set; }

        public decimal AliciTutar { get; set; }
    }
}