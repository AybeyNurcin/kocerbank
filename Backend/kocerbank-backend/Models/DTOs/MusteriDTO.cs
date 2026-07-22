namespace kocerbank_backend.Models.DTOs;
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
    public byte Cinsiyet{ get; set; }
    public int VKN { get; set; }
    public byte MusteriTipi { get; set; }
    public string SubeKodu { get; set; }
    public byte DurumKodu { get; set; }
    public DateTime KayitOlusturmaTarihi { get; set; }
    public string RecordUser { get; set; }  
    public DateTime RecordDate { get; set; }
}