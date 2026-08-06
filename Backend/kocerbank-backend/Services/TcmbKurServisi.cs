using System.Globalization;
using System.Xml.Linq;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class TcmbKurServisi
    {
        private static readonly string[] TakipEdilenDovizKodlari = { "USD", "EUR" };

        private readonly HttpClient _httpClient;

        public TcmbKurServisi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DovizKuruDosyasiDTO> GuncelKurlariGetirAsync(
            CancellationToken cancellationToken = default
        )
        {
            string xml = await _httpClient.GetStringAsync(
                "kurlar/today.xml",
                cancellationToken
            );

            XDocument belge = XDocument.Parse(xml);

            XElement kok = belge.Root
                ?? throw new InvalidOperationException(
                    "TCMB kur XML'i boş veya geçersiz."
                );

            string? tarihMetni = kok.Attribute("Tarih")?.Value;

            if (
                string.IsNullOrWhiteSpace(tarihMetni) ||
                !DateTime.TryParseExact(
                    tarihMetni,
                    "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime kurTarihi
                )
            )
            {
                throw new InvalidOperationException(
                    $"TCMB kur XML'inden tarih okunamadı: '{tarihMetni}'."
                );
            }

            DovizKuruDosyasiDTO kurDosyasi = new DovizKuruDosyasiDTO
            {
                KurTarihi = kurTarihi
            };

            foreach (string dovizKodu in TakipEdilenDovizKodlari)
            {
                XElement dovizElementi = kok
                    .Elements("Currency")
                    .FirstOrDefault(e => e.Attribute("Kod")?.Value == dovizKodu)
                    ?? throw new InvalidOperationException(
                        $"TCMB kur XML'inde '{dovizKodu}' bulunamadı."
                    );

                decimal alis = DecimalDegeriniOku(dovizElementi, "ForexBuying", dovizKodu);
                decimal satis = DecimalDegeriniOku(dovizElementi, "ForexSelling", dovizKodu);

                kurDosyasi.Kurlar[dovizKodu] = new DovizKuruDTO
                {
                    Alis = alis,
                    Satis = satis
                };
            }

            return kurDosyasi;
        }

        private static decimal DecimalDegeriniOku(
            XElement dovizElementi,
            string elementAdi,
            string dovizKodu
        )
        {
            string? metin = dovizElementi.Element(elementAdi)?.Value;

            if (
                string.IsNullOrWhiteSpace(metin) ||
                !decimal.TryParse(
                    metin,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out decimal deger
                ) ||
                deger <= 0
            )
            {
                throw new InvalidOperationException(
                    $"TCMB kur XML'inde '{dovizKodu}' için geçersiz '{elementAdi}' değeri: '{metin}'."
                );
            }

            return deger;
        }
    }
}
