namespace kocerbank_backend.Models.DTOs;
public class SubeDTO
{
    public long Id { get; set; }

    public string SubeAdi { get; set; }

    public string SubeKodu { get; set; }

    public string SubeTelefonNo { get; set; }

    public string SubeAdres { get; set; }

    public byte SubeDurumKodu { get; set; }

    public string RecordUser { get; set; }

    public DateTime RecordDate { get; set; }
}

