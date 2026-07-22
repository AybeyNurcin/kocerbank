namespace kocerbank_backend.Models.DTOs;

public class PersonelDTO
{
    public long Id { get; set; }
    public string Ad { get; set; } 
    public string Soyad { get; set; }  
    public string Rol { get; set; }
    public string Sicil { get; set; }
    public string Sifre { get; set; }
    public string TCKN { get; set; }
    public string Adres { get; set; }
    public string SubeKodu { get; set; }
    public string Email { get; set; }  
    public string TelefonNo { get; set; }
    public byte DurumKodu { get; set; }
    public string RecordUser { get; set; }  
    public DateTime RecordDate { get; set; }
}