using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
    public class MusteriDTO
    {
        public long Id { get; set; }
        public string Ad { get; set; } 
        public string Soyad { get; set; }
        public string Eposta { get; set; }
        public string Sifre { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string TelefonNo { get; set; }
        public int TCKN { get; set; }
        public CinsiyetDurumlari Cinsiyet { get; set; }
        public int VKN { get; set; }
        public MusteriTipiDurumlari MusteriTipi { get; set; }
        public string SubeSubeKodu { get; set; }
        public AktifPasifDurumlari DurumKodu { get; set; }
        public string Unvan { get; set; }
        public DateTime KayitOlusturmaTarihi { get; set; }
        public string RecordUser { get; set; }  
        public DateTime RecordDate { get; set; }
    }
}

