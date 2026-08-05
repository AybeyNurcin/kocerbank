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


        public ParaTransferServis(
            HesapRepository hesapRepository,
            MusteriRepository musteriRepository,
            ParaTransferRepository paraTransferRepository,
            DovizKuruServis dovizKuruService
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


            /*
             * IBAN KONTROLLERİ
             */

            IbanKontrolEt(
                dto.GonderenIBAN,
                "Gönderen"
            );

            IbanKontrolEt(
                dto.AliciIBAN,
                "Alıcı"
            );


            /*
             * IBAN'LARI TEMİZLE
             */

            dto.GonderenIBAN =
                IbanTemizle(
                    dto.GonderenIBAN
                );

            dto.AliciIBAN =
                IbanTemizle(
                    dto.AliciIBAN
                );


            if (
                dto.GonderenIBAN ==
                dto.AliciIBAN
            )
            {
                throw new ArgumentException(
                    "Gönderen ve alıcı IBAN aynı olamaz."
                );
            }


            /*
             * HESAPLARI GETİR
             */

            HesapDTO? gonderenHesap =
                _hesapRepository.GetirByIBAN(
                    dto.GonderenIBAN
                );

            if (gonderenHesap is null)
            {
                throw new KeyNotFoundException(
                    "Gönderen IBAN'a ait hesap bulunamadı."
                );
            }


            HesapDTO? aliciHesap =
                _hesapRepository.GetirByIBAN(
                    dto.AliciIBAN
                );

            if (aliciHesap is null)
            {
                throw new KeyNotFoundException(
                    "Alıcı IBAN'a ait hesap bulunamadı."
                );
            }


            /*
             * HESAP DURUMLARINI KONTROL ET
             */

            HesapDurumlariniKontrolEt(
                gonderenHesap,
                aliciHesap
            );


            /*
             * TRANSFER TİPİ GÖNDERİLMİŞSE
             * HAVALE/VİRMAN KONTROLÜ YAP
             *
             * TransferTipi 0 gelirse bilgi getirme
             * işlemi yine çalışabilir.
             */

            if (
                dto.TransferTipi ==
                    TransferTipleri.Havale ||
                dto.TransferTipi ==
                    TransferTipleri.Virman
            )
            {
                TransferTipiniKontrolEt(
                    dto.TransferTipi,
                    gonderenHesap,
                    aliciHesap
                );
            }
            else if (
                dto.TransferTipi !=
                TransferTipleri.None
            )
            {
                throw new ArgumentException(
                    "Transfer tipi geçersizdir."
                );
            }


            /*
             * TUTAR GÖNDERİLMİŞSE
             * BAKİYE VE TUTAR KONTROLLERİ
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
         * GERÇEK PARA TRANSFERİ
         *
         * Bu metot repository ve prosedür çağırır.
         */

        public ParaTransferDTO ParaTransferiYap(
            ParaTransferDTO dto
        )
        {
            TransferBilgileriniKontrolEt(dto);


            dto.GonderenIBAN =
                IbanTemizle(
                    dto.GonderenIBAN
                );

            dto.AliciIBAN =
                IbanTemizle(
                    dto.AliciIBAN
                );


            if (
                dto.GonderenIBAN ==
                dto.AliciIBAN
            )
            {
                throw new ArgumentException(
                    "Gönderen ve alıcı IBAN aynı olamaz."
                );
            }


            /*
             * HESAPLARI GETİR
             */

            HesapDTO? gonderenHesap =
                _hesapRepository.GetirByIBAN(
                    dto.GonderenIBAN
                );

            if (gonderenHesap is null)
            {
                throw new KeyNotFoundException(
                    "Gönderen IBAN'a ait hesap bulunamadı."
                );
            }


            HesapDTO? aliciHesap =
                _hesapRepository.GetirByIBAN(
                    dto.AliciIBAN
                );

            if (aliciHesap is null)
            {
                throw new KeyNotFoundException(
                    "Alıcı IBAN'a ait hesap bulunamadı."
                );
            }


            /*
             * HESAP DURUMLARI
             */

            HesapDurumlariniKontrolEt(
                gonderenHesap,
                aliciHesap
            );


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
             * HAVALE / VİRMAN
             */

            TransferTipiniKontrolEt(
                dto.TransferTipi,
                gonderenHesap,
                aliciHesap
            );


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

            dto.RecordUser =
                string.IsNullOrWhiteSpace(
                    dto.RecordUser
                )
                    ? null
                    : dto.RecordUser.Trim();


            return _paraTransferRepository
                .ParaTransferiYap(dto);
        }


        /*
         * GERÇEK TRANSFER İSTEĞİ KONTROLLERİ
         */

        private void TransferBilgileriniKontrolEt(
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


            IbanKontrolEt(
                dto.GonderenIBAN,
                "Gönderen"
            );

            IbanKontrolEt(
                dto.AliciIBAN,
                "Alıcı"
            );


            if (
                dto.TransferTipi !=
                    TransferTipleri.Havale &&
                dto.TransferTipi !=
                    TransferTipleri.Virman
            )
            {
                throw new ArgumentException(
                    "Transfer tipi havale veya virman olmalıdır."
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


            if (
                dto.RecordUser is not null &&
                dto.RecordUser.Trim().Length > 10
            )
            {
                throw new ArgumentException(
                    "İşlemi yapan kullanıcı en fazla 10 karakter olabilir."
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
         */

        private void IbanKontrolEt(
            string iban,
            string alanAdi
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
         * HAVALE / VİRMAN KONTROLÜ
         */

        private void TransferTipiniKontrolEt(
            TransferTipleri transferTipi,
            HesapDTO gonderenHesap,
            HesapDTO aliciHesap
        )
        {
            bool ayniMusteri =
                gonderenHesap.MusteriBilgileriId ==
                aliciHesap.MusteriBilgileriId;


            if (
                transferTipi ==
                    TransferTipleri.Havale &&
                ayniMusteri
            )
            {
                throw new InvalidOperationException(
                    "Aynı müşterinin hesapları arasında havale yapılamaz. Virman seçiniz."
                );
            }


            if (
                transferTipi ==
                    TransferTipleri.Virman &&
                !ayniMusteri
            )
            {
                throw new InvalidOperationException(
                    "Farklı müşterilerin hesapları arasında virman yapılamaz."
                );
            }
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