using System.Text.Json;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class DovizKuruServis
    {
        private readonly string _kurDosyasiYolu;

        public DovizKuruServis(
            IWebHostEnvironment environment
        )
        {
            _kurDosyasiYolu =
                Path.Combine(
                    environment.ContentRootPath,
                    "MockData",
                    "kur.json"
                );
        }


        /*
         * Transfer işleminde kullanılacak kuru döndürür.
         *
         * Dönen kurun anlamı:
         *
         * 1 gönderen dövizi = X alıcı dövizi
         */

        public decimal TransferKuruGetir(
            DovizCinsiDurumlari gonderenDoviz,
            DovizCinsiDurumlari aliciDoviz
        )
        {
            DovizCinsiniKontrolEt(
                gonderenDoviz,
                "Gönderen"
            );

            DovizCinsiniKontrolEt(
                aliciDoviz,
                "Alıcı"
            );


            // Aynı döviz cinsinde kur 1'dir.

            if (gonderenDoviz == aliciDoviz)
            {
                return 1m;
            }


            DovizKuruDosyasiDTO kurDosyasi =
                KurDosyasiniOku();


            /*
            * TL → USD/EUR
            *
            * Hedef dövizin satış kuru kullanılır.
            */

            if (
                gonderenDoviz ==
                DovizCinsiDurumlari.TL
            )
            {
                decimal hedefDovizSatisKuru =
                    SatisKuruGetir(
                        kurDosyasi,
                        aliciDoviz
                    );

                return SekizHaneYuvarla(
                    1m / hedefDovizSatisKuru
                );
            }


            /*
            * USD/EUR → TL
            *
            * Kaynak dövizin alış kuru kullanılır.
            */

            if (
                aliciDoviz ==
                DovizCinsiDurumlari.TL
            )
            {
                return AlisKuruGetir(
                    kurDosyasi,
                    gonderenDoviz
                );
            }


            /*
            * USD → EUR veya EUR → USD
            */

            decimal gonderenAlisKuru =
                AlisKuruGetir(
                    kurDosyasi,
                    gonderenDoviz
                );

            decimal hedefDovizSatisKuruCapraz =
                SatisKuruGetir(
                    kurDosyasi,
                    aliciDoviz
                );

            return SekizHaneYuvarla(
                gonderenAlisKuru /
                hedefDovizSatisKuruCapraz
            );
        }


        private DovizKuruDosyasiDTO KurDosyasiniOku()
        {
            if (!File.Exists(_kurDosyasiYolu))
            {
                throw new FileNotFoundException(
                    $"Kur dosyası bulunamadı: {_kurDosyasiYolu}"
                );
            }

            string json =
                File.ReadAllText(
                    _kurDosyasiYolu
                );

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidOperationException(
                    "Kur dosyası boş olamaz."
                );
            }

            JsonSerializerOptions options =
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

            DovizKuruDosyasiDTO? kurDosyasi =
                JsonSerializer.Deserialize
                    <DovizKuruDosyasiDTO>(
                        json,
                        options
                    );

            if (kurDosyasi is null)
            {
                throw new InvalidOperationException(
                    "Kur dosyası okunamadı."
                );
            }

            if (
                kurDosyasi.Kurlar is null ||
                kurDosyasi.Kurlar.Count == 0
            )
            {
                throw new InvalidOperationException(
                    "Kur dosyasında döviz kuru bulunamadı."
                );
            }

            return kurDosyasi;
        }


        private decimal AlisKuruGetir(
            DovizKuruDosyasiDTO kurDosyasi,
            DovizCinsiDurumlari dovizCinsi
        )
        {
            if (dovizCinsi == DovizCinsiDurumlari.TL)
            {
                return 1m;
            }

            DovizKuruDTO kur =
                DovizKurunuBul(
                    kurDosyasi,
                    dovizCinsi
                );

            if (kur.Alis <= 0)
            {
                throw new InvalidOperationException(
                    $"{dovizCinsi} alış kuru geçersizdir."
                );
            }

            return kur.Alis;
        }


        private decimal SatisKuruGetir(
            DovizKuruDosyasiDTO kurDosyasi,
            DovizCinsiDurumlari dovizCinsi
        )
        {
            if (dovizCinsi == DovizCinsiDurumlari.TL)
            {
                return 1m;
            }

            DovizKuruDTO kur =
                DovizKurunuBul(
                    kurDosyasi,
                    dovizCinsi
                );

            if (kur.Satis <= 0)
            {
                throw new InvalidOperationException(
                    $"{dovizCinsi} satış kuru geçersizdir."
                );
            }

            return kur.Satis;
        }


        private DovizKuruDTO DovizKurunuBul(
            DovizKuruDosyasiDTO kurDosyasi,
            DovizCinsiDurumlari dovizCinsi
        )
        {
            string dovizKodu =
                dovizCinsi.ToString()
                    .ToUpperInvariant();

            bool bulundu =
                kurDosyasi.Kurlar.TryGetValue(
                    dovizKodu,
                    out DovizKuruDTO? kur
                );

            if (!bulundu || kur is null)
            {
                throw new KeyNotFoundException(
                    $"{dovizKodu} kuru bulunamadı."
                );
            }

            return kur;
        }


        private void DovizCinsiniKontrolEt(
            DovizCinsiDurumlari dovizCinsi,
            string alanAdi
        )
        {
            if (
                dovizCinsi ==
                DovizCinsiDurumlari.None
            )
            {
                throw new ArgumentException(
                    $"{alanAdi} döviz cinsi seçilmelidir."
                );
            }

            if (
                !Enum.IsDefined(
                    typeof(DovizCinsiDurumlari),
                    dovizCinsi
                )
            )
            {
                throw new ArgumentException(
                    $"{alanAdi} döviz cinsi geçersizdir."
                );
            }
        }


        private decimal SekizHaneYuvarla(
            decimal deger
        )
        {
            return decimal.Round(
                deger,
                8,
                MidpointRounding.AwayFromZero
            );
        }

        public DateTime KurTarihiniGetir()
        {
            DovizKuruDosyasiDTO kurDosyasi =
                KurDosyasiniOku();

            return kurDosyasi.KurTarihi;
        }
    }
}