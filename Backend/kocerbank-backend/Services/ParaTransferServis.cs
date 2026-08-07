using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class ParaTransferServis
    {
        private readonly HesapRepository
            _hesapRepository;

        private readonly MusteriRepository
            _musteriRepository;

        private readonly ParaTransferRepository
            _paraTransferRepository;

        private readonly DovizKuruServis
            _dovizKuruService;

        private readonly AktifPersonelServis
            _aktifPersonelServis;


        public ParaTransferServis(
            HesapRepository hesapRepository,
            MusteriRepository musteriRepository,
            ParaTransferRepository paraTransferRepository,
            DovizKuruServis dovizKuruService,
            AktifPersonelServis aktifPersonelServis
        )
        {
            _hesapRepository =
                hesapRepository;

            _musteriRepository =
                musteriRepository;

            _paraTransferRepository =
                paraTransferRepository;

            _dovizKuruService =
                dovizKuruService;

            _aktifPersonelServis =
                aktifPersonelServis;
        }


        /*
         * TRANSFER BİLGİLERİNİ GETİR
         *
         * Bu metot prosedür çağırmaz.
         * Bakiye değiştirmez.
         * Transfer kaydı oluşturmaz.
         */

        public ParaTransferDTO TransferBilgileriniGetir(
            ParaTransferDTO dto
        )
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Transfer bilgileri gönderilmelidir."
                );
            }


            (
                string gonderenIban,
                string aliciIban,
                HesapDTO gonderenHesap,
                HesapDTO aliciHesap,
                TransferTipleri transferTipi
            ) =
                HesaplariGetirVeDogrula(
                    dto.GonderenIBAN,
                    dto.AliciIBAN,
                    dto.TransferKanali
                );

            dto.GonderenIBAN =
                gonderenIban;

            dto.AliciIBAN =
                aliciIban;

            dto.TransferTipi =
                transferTipi;


            /*
             * TUTAR GÖNDERİLMİŞSE
             * BAKİYE VE TUTAR KONTROLLERİ
             *
             * Kullanıcı henüz tutar girmeden de
             * hesap ve kural kontrolü yapılabilsin
             * diye tutar 0 gelirse atlanır.
             */

            if (dto.GonderenTutar > 0)
            {
                TutarKontrolEt(
                    dto.GonderenTutar
                );

                if (
                    gonderenHesap.Bakiye <
                    dto.GonderenTutar
                )
                {
                    throw new InvalidOperationException(
                        "Gönderen hesap bakiyesi yetersizdir."
                    );
                }
            }


            /*
             * HESAP SAHİPLERİNİ GETİR
             */

            MusteriDTO? gonderenMusteri =
                _musteriRepository.GetirById(
                    gonderenHesap.MusteriBilgileriId
                );

            if (gonderenMusteri is null)
            {
                throw new KeyNotFoundException(
                    "Gönderen hesap sahibi bulunamadı."
                );
            }


            MusteriDTO? aliciMusteri =
                _musteriRepository.GetirById(
                    aliciHesap.MusteriBilgileriId
                );

            if (aliciMusteri is null)
            {
                throw new KeyNotFoundException(
                    "Alıcı hesap sahibi bulunamadı."
                );
            }


            /*
             * ID VE DÖVİZ BİLGİLERİNİ DTO'YA YAZ
             */

            dto.GonderenHesapId =
                gonderenHesap.Id;

            dto.AliciHesapId =
                aliciHesap.Id;

            dto.GonderenDovizTipi =
                gonderenHesap.DovizCinsi;

            dto.AliciDovizTipi =
                aliciHesap.DovizCinsi;


            /*
             * KURU HESAPLA
             *
             * Anlamı:
             * 1 gönderen dövizi =
             * X alıcı dövizi
             */

            dto.DovizKuru =
                _dovizKuruService
                    .TransferKuruGetir(
                        gonderenHesap.DovizCinsi,
                        aliciHesap.DovizCinsi
                    );


            dto.KurAciklamasi =
                $"1 {gonderenHesap.DovizCinsi} = " +
                $"{dto.DovizKuru} " +
                $"{aliciHesap.DovizCinsi}";


            dto.KurTarihi =
                _dovizKuruService
                    .KurTarihiniGetir();


            /*
             * DETAYLI HESAP BİLGİLERİNİ DTO'YA YAZ
             */

            dto.GonderenHesap =
                TransferHesabaDonustur(
                    gonderenHesap,
                    gonderenMusteri
                );

            dto.AliciHesap =
                TransferHesabaDonustur(
                    aliciHesap,
                    aliciMusteri
                );


            /*
             * TUTAR VARSA ALICIYA GEÇECEK
             * TUTARI ÖNİZLEME İÇİN HESAPLA
             */

            if (dto.GonderenTutar > 0)
            {
                dto.AliciTutar =
                    decimal.Round(
                        dto.GonderenTutar *
                        dto.DovizKuru,
                        2,
                        MidpointRounding.AwayFromZero
                    );
            }


            return dto;
        }


        /*
         * TEK HESABIN BİLGİLERİNİ GETİR
         *
         * Karşı taraf IBAN'ı beklenmeden,
         * bir IBAN'ın hesap sahibi kontrolü
         * için kullanılır. Karşı taraf henüz
         * bilinmediğinden Havale/Virman/SWIFT
         * kuralı burada değerlendirilemez;
         * yalnızca IBAN biçimi ve hesabın
         * varlığı/aktifliği kontrol edilir.
         *
         * Bu metot prosedür çağırmaz.
         * Bakiye değiştirmez.
         * Transfer kaydı oluşturmaz.
         */

        public TransferHesapDTO TekHesapBilgisiGetir(
            string iban,
            TransferKanallari kanal
        )
        {
            IbanKontrolEt(
                iban,
                "IBAN",
                kanal
            );

            string temizIban =
                IbanTemizle(iban);

            HesapDTO? hesap =
                _hesapRepository.GetirByIBAN(
                    temizIban
                );

            if (hesap is null)
            {
                throw new KeyNotFoundException(
                    "IBAN'a ait hesap bulunamadı."
                );
            }

            if (
                hesap.HesapDurumKodu !=
                HesapDurumKodlari.Aktif
            )
            {
                throw new InvalidOperationException(
                    "Hesap aktif değildir."
                );
            }

            MusteriDTO? musteri =
                _musteriRepository.GetirById(
                    hesap.MusteriBilgileriId
                );

            if (musteri is null)
            {
                throw new KeyNotFoundException(
                    "Hesap sahibi bulunamadı."
                );
            }

            return TransferHesabaDonustur(
                hesap,
                musteri
            );
        }


        /*
         * GERÇEK PARA TRANSFERİ
         *
         * Bu metot repository ve prosedür çağırır.
         */

        public ParaTransferDTO ParaTransferiYap(
            ParaTransferDTO dto
        )
        {
            TransferIstegiGenelKontrolEt(dto);


            (
                string gonderenIban,
                string aliciIban,
                HesapDTO gonderenHesap,
                HesapDTO aliciHesap,
                TransferTipleri transferTipi
            ) =
                HesaplariGetirVeDogrula(
                    dto.GonderenIBAN,
                    dto.AliciIBAN,
                    dto.TransferKanali
                );

            dto.GonderenIBAN =
                gonderenIban;

            dto.AliciIBAN =
                aliciIban;

            dto.TransferTipi =
                transferTipi;


            /*
             * BAKİYE
             */

            if (
                gonderenHesap.Bakiye <
                dto.GonderenTutar
            )
            {
                throw new InvalidOperationException(
                    "Gönderen hesap bakiyesi yetersizdir."
                );
            }


            /*
             * GÜVENİLİR HESAP BİLGİLERİNİ
             * DTO'YA YENİDEN YAZ
             *
             * Frontend'den gelen ID, döviz ve kur
             * değerlerine güvenilmez.
             */

            dto.GonderenHesapId =
                gonderenHesap.Id;

            dto.AliciHesapId =
                aliciHesap.Id;

            dto.GonderenDovizTipi =
                gonderenHesap.DovizCinsi;

            dto.AliciDovizTipi =
                aliciHesap.DovizCinsi;


            dto.DovizKuru =
                _dovizKuruService
                    .TransferKuruGetir(
                        gonderenHesap.DovizCinsi,
                        aliciHesap.DovizCinsi
                    );


            dto.Aciklama =
                string.IsNullOrWhiteSpace(
                    dto.Aciklama
                )
                    ? null
                    : dto.Aciklama.Trim();

            // Frontend'den gelen RecordUser dikkate alınmaz.
            // Giriş yapan personelin sicili backend tarafından atanır.
            dto.RecordUser =
                _aktifPersonelServis.SicilNoGetir();


            return _paraTransferRepository
                .ParaTransferiYap(dto);
        }


        /*
         * IBAN'LARI DOĞRULA, HESAPLARI GETİR,
         * TRANSFER KURALINI UYGULA
         *
         * TransferBilgileriniGetir ve ParaTransferiYap
         * tarafından ortak kullanılır. Gönderen/alıcı
         * hesapları getirir, aktif olduklarını
         * doğrular ve kanal + sahiplik + döviz
         * bilgisine göre işlemin Havale/EFT, Virman
         * veya SWIFT kuralına uygun olup olmadığını
         * kontrol edip gerçek transfer tipini döner.
         */

        private (
            string GonderenIBAN,
            string AliciIBAN,
            HesapDTO GonderenHesap,
            HesapDTO AliciHesap,
            TransferTipleri TransferTipi
        ) HesaplariGetirVeDogrula(
            string gonderenIbanHam,
            string aliciIbanHam,
            TransferKanallari kanal
        )
        {
            IbanKontrolEt(
                gonderenIbanHam,
                "Gönderen",
                kanal
            );

            IbanKontrolEt(
                aliciIbanHam,
                "Alıcı",
                kanal
            );

            string gonderenIban =
                IbanTemizle(gonderenIbanHam);

            string aliciIban =
                IbanTemizle(aliciIbanHam);

            if (gonderenIban == aliciIban)
            {
                throw new ArgumentException(
                    "Gönderen ve alıcı IBAN aynı olamaz."
                );
            }


            HesapDTO? gonderenHesap =
                _hesapRepository.GetirByIBAN(
                    gonderenIban
                );

            if (gonderenHesap is null)
            {
                throw new KeyNotFoundException(
                    "Gönderen IBAN'a ait hesap bulunamadı."
                );
            }


            HesapDTO? aliciHesap =
                _hesapRepository.GetirByIBAN(
                    aliciIban
                );

            if (aliciHesap is null)
            {
                throw new KeyNotFoundException(
                    "Alıcı IBAN'a ait hesap bulunamadı."
                );
            }


            HesapDurumlariniKontrolEt(
                gonderenHesap,
                aliciHesap
            );


            TransferTipleri transferTipi =
                TransferKuraliniDogrula(
                    kanal,
                    gonderenHesap,
                    aliciHesap
                );


            return (
                gonderenIban,
                aliciIban,
                gonderenHesap,
                aliciHesap,
                transferTipi
            );
        }


        /*
         * TRANSFER KURALI
         *
         * Havale/EFT : Farklı müşterilerin TL
         *              hesapları arasında yapılır.
         *
         * Virman     : Aynı müşterinin TL hesapları
         *              arasında yapılır.
         *
         * SWIFT      : Yukarıdaki iki durumun
         *              dışında kalan, yani TL-TL
         *              olmayan tüm transferlerde
         *              kullanılır (sahiplik fark
         *              etmez).
         *
         * Bu metot, hesapların gerçek sahiplik ve
         * döviz bilgisine göre işlemi doğrular ve
         * prosedüre/DB'ye yazılacak gerçek transfer
         * tipini (Havale/Virman) döner. Frontend'den
         * gelen TransferTipi değerine güvenilmez.
         */

        private TransferTipleri TransferKuraliniDogrula(
            TransferKanallari kanal,
            HesapDTO gonderenHesap,
            HesapDTO aliciHesap
        )
        {
            bool ayniMusteri =
                gonderenHesap.MusteriBilgileriId ==
                aliciHesap.MusteriBilgileriId;

            bool ikisiDeTL =
                gonderenHesap.DovizCinsi ==
                    DovizCinsiDurumlari.TL &&
                aliciHesap.DovizCinsi ==
                    DovizCinsiDurumlari.TL;


            if (kanal == TransferKanallari.HavaleEft)
            {
                if (!ikisiDeTL)
                {
                    throw new ArgumentException(
                        "Havale/EFT işlemi yalnızca TL hesaplar arasında yapılabilir. Farklı döviz cinsleri için SWIFT ekranını kullanınız."
                    );
                }

                if (ayniMusteri)
                {
                    throw new InvalidOperationException(
                        "Aynı müşterinin TL hesapları arasındaki transferler Virman işlemidir. Virman ekranını kullanınız."
                    );
                }

                return TransferTipleri.Havale;
            }


            if (kanal == TransferKanallari.Virman)
            {
                if (!ikisiDeTL)
                {
                    throw new ArgumentException(
                        "Virman işlemi yalnızca TL hesaplar arasında yapılabilir. Farklı döviz cinsleri için SWIFT ekranını kullanınız."
                    );
                }

                if (!ayniMusteri)
                {
                    throw new InvalidOperationException(
                        "Virman işlemi yalnızca aynı müşterinin hesapları arasında yapılabilir. Farklı müşteriler için Havale/EFT ekranını kullanınız."
                    );
                }

                return TransferTipleri.Virman;
            }


            if (kanal == TransferKanallari.Swift)
            {
                if (ikisiDeTL)
                {
                    throw new ArgumentException(
                        ayniMusteri
                            ? "Aynı müşterinin TL hesapları arasındaki transferler Virman işlemidir. Virman ekranını kullanınız."
                            : "Farklı müşterilerin TL hesapları arasındaki transferler Havale/EFT işlemidir. Havale/EFT ekranını kullanınız."
                    );
                }

                return ayniMusteri
                    ? TransferTipleri.Virman
                    : TransferTipleri.Havale;
            }


            throw new ArgumentException(
                "Transfer kanalı geçersizdir."
            );
        }


        /*
         * GERÇEK TRANSFER İSTEĞİNİN GENEL
         * ALAN KONTROLLERİ
         *
         * Hesap/kural kontrolleri
         * HesaplariGetirVeDogrula içinde yapılır.
         */

        private void TransferIstegiGenelKontrolEt(
            ParaTransferDTO dto
        )
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Transfer bilgileri gönderilmelidir."
                );
            }


            TutarKontrolEt(
                dto.GonderenTutar
            );


            if (
                dto.Aciklama is not null &&
                dto.Aciklama.Trim().Length > 100
            )
            {
                throw new ArgumentException(
                    "Açıklama en fazla 100 karakter olabilir."
                );
            }
        }


        /*
         * ORTAK TUTAR KONTROLÜ
         */

        private void TutarKontrolEt(
            decimal gonderenTutar
        )
        {
            if (gonderenTutar <= 0)
            {
                throw new ArgumentException(
                    "Gönderen tutar sıfırdan büyük olmalıdır."
                );
            }


            if (
                gonderenTutar >
                999999999999.99m
            )
            {
                throw new ArgumentException(
                    "Gönderen tutar izin verilen üst sınırı aşmaktadır."
                );
            }


            if (
                OndalikBasamakSayisiGetir(
                    gonderenTutar
                ) > 2
            )
            {
                throw new ArgumentException(
                    "Gönderen tutar en fazla iki ondalık basamak içerebilir."
                );
            }
        }


        /*
         * ORTAK HESAP DURUM KONTROLÜ
         */

        private void HesapDurumlariniKontrolEt(
            HesapDTO gonderenHesap,
            HesapDTO aliciHesap
        )
        {
            if (
                gonderenHesap.HesapDurumKodu !=
                HesapDurumKodlari.Aktif
            )
            {
                throw new InvalidOperationException(
                    "Gönderen hesap aktif değildir."
                );
            }


            if (
                aliciHesap.HesapDurumKodu !=
                HesapDurumKodlari.Aktif
            )
            {
                throw new InvalidOperationException(
                    "Alıcı hesap aktif değildir."
                );
            }
        }


        /*
         * IBAN KONTROLÜ
         *
         * Havale/EFT yalnızca yurt içi TR IBAN
         * kabul eder. SWIFT'te yabancı IBAN'lar
         * da girilebildiği için genel IBAN
         * biçimi yeterli kabul edilir.
         */

        private void IbanKontrolEt(
            string iban,
            string alanAdi,
            TransferKanallari kanal
        )
        {
            if (string.IsNullOrWhiteSpace(iban))
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN girilmesi zorunludur."
                );
            }


            string temizIban =
                IbanTemizle(iban);


            if (kanal == TransferKanallari.Swift)
            {
                GenelIbanKontrolEt(
                    temizIban,
                    alanAdi
                );

                return;
            }


            if (temizIban.Length != 26)
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN 26 karakter olmalıdır."
                );
            }


            if (
                !temizIban.StartsWith(
                    "TR",
                    StringComparison.Ordinal
                )
            )
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN 'TR' ile başlamalıdır."
                );
            }


            string sayisalKisim =
                temizIban.Substring(2);


            if (!sayisalKisim.All(char.IsDigit))
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN, 'TR' sonrasında yalnızca rakamlardan oluşmalıdır."
                );
            }
        }


        /*
         * GENEL (YABANCI) IBAN KONTROLÜ
         *
         * ISO 13616: 2 harfli ülke kodu +
         * 2 haneli kontrol basamağı +
         * 11-30 alfanümerik karakter (BBAN).
         */

        private void GenelIbanKontrolEt(
            string temizIban,
            string alanAdi
        )
        {
            if (
                temizIban.Length < 15 ||
                temizIban.Length > 34
            )
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN 15-34 karakter arasında olmalıdır."
                );
            }


            if (
                !char.IsLetter(temizIban[0]) ||
                !char.IsLetter(temizIban[1])
            )
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN, iki harfli bir ülke kodu ile başlamalıdır."
                );
            }


            if (
                !char.IsDigit(temizIban[2]) ||
                !char.IsDigit(temizIban[3])
            )
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN, ülke kodundan sonra iki haneli kontrol basamağı içermelidir."
                );
            }


            if (
                !temizIban
                    .Substring(4)
                    .All(char.IsLetterOrDigit)
            )
            {
                throw new ArgumentException(
                    $"{alanAdi} IBAN yalnızca harf ve rakamlardan oluşmalıdır."
                );
            }
        }


        /*
         * IBAN TEMİZLEME
         */

        private string IbanTemizle(
            string iban
        )
        {
            return iban
                .Replace(" ", string.Empty)
                .Trim()
                .ToUpperInvariant();
        }


        /*
         * HESAP DTO → TRANSFER HESAP DTO
         */

        private TransferHesapDTO TransferHesabaDonustur(
            HesapDTO hesap,
            MusteriDTO musteri
        )
        {
            string hesapSahibi;


            if (
                musteri.MusteriTipi ==
                    MusteriTipiDurumlari.Kurumsal &&
                !string.IsNullOrWhiteSpace(
                    musteri.Unvan
                )
            )
            {
                hesapSahibi =
                    musteri.Unvan.Trim();
            }
            else
            {
                hesapSahibi =
                    $"{musteri.Ad} {musteri.Soyad}"
                        .Trim();
            }


            return new TransferHesapDTO
            {
                Id =
                    hesap.Id,

                RecordUser =
                    hesap.RecordUser,

                RecordDate =
                    hesap.RecordDate,

                HesapAdi =
                    hesap.HesapAdi,

                HesapNo =
                    hesap.HesapNo,

                IBAN =
                    hesap.IBAN,

                Bakiye =
                    hesap.Bakiye,

                SubeSubeKodu =
                    hesap.SubeSubeKodu,

                DovizCinsi =
                    hesap.DovizCinsi,

                HesapAcilisTarihi =
                    hesap.HesapAcilisTarihi,

                HesapDurumKodu =
                    hesap.HesapDurumKodu,

                MusteriBilgileriId =
                    hesap.MusteriBilgileriId,

                HesapTipi =
                    hesap.HesapTipi,

                HesapSahibi =
                    hesapSahibi
            };
        }


        /*
         * ONDALIK BASAMAK SAYISI
         */

        private int OndalikBasamakSayisiGetir(
            decimal deger
        )
        {
            int[] parcalar =
                decimal.GetBits(deger);

            return (
                parcalar[3] >> 16
            ) & 0x7F;
        }
    }
}
