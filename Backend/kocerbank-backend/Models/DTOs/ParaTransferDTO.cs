using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class ParaTransferDTO
    {
        /*
         * FRONTEND TARAFINDAN GÖNDERİLECEK
         */

        public string GonderenIBAN { get; set; } =
            string.Empty;

        public string AliciIBAN { get; set; } =
            string.Empty;

        public TransferTipleri TransferTipi
        {
            get;
            set;
        }

        public TransferKanallari TransferKanali
        {
            get;
            set;
        }

        public decimal GonderenTutar
        {
            get;
            set;
        }

        public string? Aciklama
        {
            get;
            set;
        }

        public string? RecordUser
        {
            get;
            set;
        }


        /*
         * SERVICE TARAFINDAN DOLDURULACAK
         */

        public long GonderenHesapId
        {
            get;
            set;
        }

        public long AliciHesapId
        {
            get;
            set;
        }

        public DovizCinsiDurumlari GonderenDovizTipi
        {
            get;
            set;
        }

        public DovizCinsiDurumlari AliciDovizTipi
        {
            get;
            set;
        }

        public decimal DovizKuru
        {
            get;
            set;
        }

        public HesapDTO? GonderenHesap
        {
            get;
            set;
        }

        public HesapDTO? AliciHesap
        {
            get;
            set;
        }

        public string? KurAciklamasi
        {
            get;
            set;
        }

        public DateTime? KurTarihi
        {
            get;
            set;
        }


        /*
         * PROSEDÜRDEN DÖNECEK
         */

        public long TransferId
        {
            get;
            set;
        }

        public long GonderenHareketId
        {
            get;
            set;
        }

        public long AliciHareketId
        {
            get;
            set;
        }

        public decimal GonderenYeniBakiye
        {
            get;
            set;
        }

        public decimal AliciYeniBakiye
        {
            get;
            set;
        }

        public decimal AliciTutar
        {
            get;
            set;
        }
    }


    /*
     * HESAP HAREKETİ DETAY EKRANI İÇİN
     *
     * Bir KB_PARATRANSFERI kaydının, gönderen/alıcı
     * ad-soyad ve IBAN bilgileriyle birlikte, hangi
     * kanaldan (Havale/EFT, Swift, Virman) yapıldığı
     * çözümlenmiş halidir.
     */

    public class ParaTransferiDetayDTO
    {
        public TransferKanallari TransferKanali
        {
            get;
            set;
        }

        public string GonderenAdSoyad { get; set; } =
            string.Empty;

        public string GonderenIBAN { get; set; } =
            string.Empty;

        public string AliciAdSoyad { get; set; } =
            string.Empty;

        public string AliciIBAN { get; set; } =
            string.Empty;

        public decimal Tutar
        {
            get;
            set;
        }

        public DovizCinsiDurumlari GonderenDovizCinsi
        {
            get;
            set;
        }

        public DovizCinsiDurumlari AliciDovizCinsi
        {
            get;
            set;
        }

        public decimal DovizKuru
        {
            get;
            set;
        }

        public decimal AliciTutar
        {
            get;
            set;
        }

        public string? Aciklama
        {
            get;
            set;
        }
    }
}