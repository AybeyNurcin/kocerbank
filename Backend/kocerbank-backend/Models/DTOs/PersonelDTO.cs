using kocerbank_backend.Enums;

namespace kocerbank_backend.Models.DTOs
{
public class PersonelDTO : BaseDTO
{
    public string Ad { get; set; } = string.Empty;
    public string Soyad { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Sicil { get; set; } = string.Empty;
    public string Sifre { get; set; } = string.Empty;
    public string TCKN { get; set; } = string.Empty;
    public string Adres { get; set; } = string.Empty;
    public string SubeKodu { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TelefonNo { get; set; } = string.Empty;
    public AktifPasifDurumlari DurumKodu { get; set; }
}
}