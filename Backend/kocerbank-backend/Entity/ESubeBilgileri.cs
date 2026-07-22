namespace KocerBank.Backend.Entity;

public class ESubeBilgileri
{
    public long Id { get; set; }

    public string SubeAdi { get; set; } = string.Empty;

    public string SubeKodu { get; set; } = string.Empty;

    public string SubeTelefonNo { get; set; } = string.Empty;

    public string SubeAdres { get; set; } = string.Empty;

    public byte SubeDurumKodu { get; set; }

    public string RecordUser { get; set; } = string.Empty;

    public DateTime RecordDate { get; set; }
}