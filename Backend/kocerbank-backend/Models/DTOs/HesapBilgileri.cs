using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class HesapDTO : BaseDTO
    {
        // HESAP BİLGİLERİ

        public string HesapAdi { get; set; } = string.Empty;

        public string HesapNo { get; set; } = string.Empty;

        public string IBAN { get; set; } = string.Empty;

        public decimal Bakiye { get; set; }

        public string SubeSubeKodu { get; set; } = string.Empty;

        // Görüntüleme amacıyla KB_SUBE tablosundan doldurulacak.
        public string SubeAdi { get; set; } = string.Empty;

        public DovizCinsiDurumlari DovizCinsi { get; set; }

        public DateTime HesapAcilisTarihi { get; set; }

        public HesapDurumKodlari HesapDurumKodu { get; set; }

        public long MusteriBilgileriId { get; set; }

        public HesapTipiDurumlari HesapTipi { get; set; }


        // BAŞARI EKRANI İÇİN MÜŞTERİ BİLGİLERİ

        public string HesapSahibi { get; set; } = string.Empty;

        public string? Tckn { get; set; }

        public string? Vkn { get; set; }
    }


    public class HesapAramaKriterleriDTO
    {
        public long? Id { get; set; }

        public string? HesapAdi { get; set; }

        public string? HesapNo { get; set; }

        public string? IBAN { get; set; }

        public decimal? Bakiye { get; set; }

        public string? SubeSubeKodu { get; set; }

        public DovizCinsiDurumlari? DovizCinsi { get; set; }

        public DateTime? HesapAcilisTarihi { get; set; }

        public HesapDurumKodlari? HesapDurumKodu { get; set; }

        public long? MusteriBilgileriId { get; set; }

        public HesapTipiDurumlari? HesapTipi { get; set; }
    }

    public class HesapDashboardDTO
    {
        public long ToplamHesap { get; set; }
        public long AktifSayi { get; set; }
        public long PasifSayi { get; set; }
        public long TlSayi { get; set; }
        public long UsdSayi { get; set; }
        public long EurSayi { get; set; }
        public string? SonHesapNo { get; set; }
    }
}