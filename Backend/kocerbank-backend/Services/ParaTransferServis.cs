using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class ParaTransferServis
    {
        private readonly HesapRepository
            _hesapRepository;

        private readonly ParaTransferRepository
            _paraTransferRepository;

        private readonly DovizKuruServis
            _dovizKuruService;


        public ParaTransferServis(
            HesapRepository hesapRepository,
            ParaTransferRepository paraTransferRepository,
            DovizKuruServis dovizKuruService
        )
        {
            _hesapRepository =
                hesapRepository;

            _paraTransferRepository =
                paraTransferRepository;

            _dovizKuruService =
                dovizKuruService;
        }


        public ParaTransferDTO ParaTransferiYap(
            ParaTransferDTO dto
        )
        {
            /*
             * 1. FRONTEND'DEN GELEN TEMEL
             * BİLGİLERİ KONTROL ET
             */

            TransferBilgileriniKontrolEt(dto);


            /*
             * 2. IBAN'LARI TEMİZLE
             */

            dto.GonderenIBAN =
                IbanTemizle(
                    dto.GonderenIBAN
                );

            dto.AliciIBAN =
                IbanTemizle(
                    dto.AliciIBAN
                );


            /*
             * 3. AYNI IBAN KONTROLÜ
             */

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
             * 4. GÖNDEREN HESABI BUL
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


            /*
             * 5. ALICI HESABI BUL
             */

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
             * 6. HESAP DURUM KONTROLLERİ
             */

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


            /*
             * 7. BAKİYE KONTROLÜ
             *
             * Bu kontrol prosedürde de tekrar yapılır.
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
             * 8. HAVALE / VİRMAN KONTROLÜ
             */

            TransferTipiniKontrolEt(
                dto.TransferTipi,
                gonderenHesap,
                aliciHesap
            );


            /*
             * 9. HESAP BİLGİLERİNİ DTO'YA AKTAR
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
             * 10. TRANSFER KURUNU HESAPLA
             */

            dto.DovizKuru =
                _dovizKuruService
                    .TransferKuruGetir(
                        gonderenHesap.DovizCinsi,
                        aliciHesap.DovizCinsi
                    );


            /*
             * 11. METİN ALANLARINI TEMİZLE
             */

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


            /*
             * 12. REPOSITORY'Yİ ÇAĞIR
             */

            return _paraTransferRepository
                .ParaTransferiYap(dto);
        }


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


            if (dto.GonderenTutar <= 0)
            {
                throw new ArgumentException(
                    "Gönderen tutar sıfırdan büyük olmalıdır."
                );
            }


            /*
             * Veritabanındaki tutar kolonu
             * NUMBER(14,2) olduğu için en fazla
             * 12 tam sayı hanesi kullanılabilir.
             */

            if (
                dto.GonderenTutar >
                999999999999.99m
            )
            {
                throw new ArgumentException(
                    "Gönderen tutar izin verilen üst sınırı aşmaktadır."
                );
            }


            if (
                OndalikBasamakSayisiGetir(
                    dto.GonderenTutar
                ) > 2
            )
            {
                throw new ArgumentException(
                    "Gönderen tutar en fazla iki ondalık basamak içerebilir."
                );
            }


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


        private string IbanTemizle(
            string iban
        )
        {
            return iban
                .Replace(" ", string.Empty)
                .Trim()
                .ToUpperInvariant();
        }


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