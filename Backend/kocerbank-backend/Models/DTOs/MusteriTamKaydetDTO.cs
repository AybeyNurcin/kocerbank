namespace kocerbank_backend.Models.DTOs
{
    public class MusteriTamKaydetDTO
    {
        public MusteriDTO Musteri { get; set; } =
            new();

        public MusteriIletisimFormDTO Iletisim { get; set; } =
            new();
    }

    public class MusteriIletisimFormDTO
    {
        public string? EvTelefonNo { get; set; }

        public string? IsTelefonNo { get; set; }

        public string? EvAdres { get; set; }

        public string? IsAdres { get; set; }
    }

    public class MusteriTamKaydetSonucDTO
    {
        public long MusteriId { get; set; }

        public long IletisimId { get; set; }

        public DateTime KayitOlusturmaTarihi {
            get;
            set;
        }
    }
}