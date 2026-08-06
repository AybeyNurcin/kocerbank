using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class HesapService
    {
        private readonly HesapRepository _hesapRepository;

        public HesapService(HesapRepository hesapRepository)
        {
            _hesapRepository = hesapRepository;
        }

        // 1. EKLEME
        public HesapDTO Ekle(HesapDTO dto)
        {
            HesapEklemeRealityCheck(dto);

            /*
            * Yeni oluşturulan bütün hesaplar
            * otomatik olarak aktif ve sıfır bakiyeli açılır.
            */
            dto.HesapDurumKodu =
                HesapDurumKodlari.Aktif;

            dto.Bakiye = 0;

            /*
            * Kullanıcının yazdığı metinlerdeki
            * baştaki ve sondaki boşlukları temizler.
            */
            dto.HesapAdi =
                dto.HesapAdi.Trim();

            dto.SubeSubeKodu =
                dto.SubeSubeKodu
                    .Trim()
                    .ToUpperInvariant();

            dto.RecordUser =
                string.IsNullOrWhiteSpace(dto.RecordUser)
                    ? null
                    : dto.RecordUser.Trim();

            return _hesapRepository.Ekle(dto);
        }

        // 2. ID'YE GÖRE GETİR
        public HesapDTO GetirById(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz hesap ID'si.");
            }

            HesapDTO? hesap =
                _hesapRepository.GetirById(id);

            if (hesap is null)
            {
                throw new KeyNotFoundException(
                    $"Hesap bulunamadı: ID = {id}");
            }

            return hesap;
        }

        // 3. LİSTELEME
        public List<HesapDTO> Listele(
            HesapAramaKriterleriDTO aramaKriterleri)
        {
            return _hesapRepository.Listele(
                aramaKriterleri);
        }

        // 4. GÜNCELLEME
        public void Guncelle(HesapDTO dto)
        {
            HesapGuncellemeRealityCheck(dto);

            HesapDTO? mevcutHesap =
                _hesapRepository.GetirById(dto.Id);

            if (mevcutHesap is null)
            {
                throw new KeyNotFoundException(
                    $"{dto.Id} ID'li hesap bulunamadı.");
            }

            _hesapRepository.Guncelle(dto);
        }

        // DASHBOARD ÖZETİ
        public HesapDashboardDTO GetirDashboardOzet()
        {
            return _hesapRepository.GetirDashboardOzet();
        }

        // 5. PARA ÇEKME / YATIRMA
        public HesapCekYatirDTO ParaCekYatir(HesapCekYatirDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "İşlem bilgileri gönderilmelidir.");
            }

            if (dto.HesapId <= 0)
            {
                throw new ArgumentException("Geçersiz hesap ID.");
            }

            if (dto.IslemTipi != HesapHareketTipleri.ParaYatirma && dto.IslemTipi != HesapHareketTipleri.ParaCekme)
            {
                throw new ArgumentException("İşlem tipi para yatırma veya para çekme olmalıdır.");
            }

            if (dto.Tutar <= 0)
            {
                throw new ArgumentException("İşlem tutarı sıfırdan büyük olmalıdır.");
            }

            if (dto.RecordUser is not null && dto.RecordUser.Length > 10)
            {
                throw new ArgumentException("İşlemi yapan kullanıcı en fazla 10 karakter olabilir.");
            }

            dto.RecordUser = string.IsNullOrWhiteSpace(dto.RecordUser) ? null : dto.RecordUser.Trim();

            return _hesapRepository.ParaCekYatir(dto);
        }

        private void HesapEklemeRealityCheck(
            HesapDTO dto
        )
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Hesap bilgileri gönderilmelidir."
                );
            }


            // HESAP ADI

            if (string.IsNullOrWhiteSpace(dto.HesapAdi))
            {
                throw new ArgumentException(
                    "Hesap adı girilmesi zorunludur."
                );
            }

            if (dto.HesapAdi.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Hesap adı en fazla 50 karakter olabilir."
                );
            }


            // ŞUBE KODU

            if (string.IsNullOrWhiteSpace(
                dto.SubeSubeKodu
            ))
            {
                throw new ArgumentException(
                    "Şube seçilmesi zorunludur."
                );
            }

            string subeKodu =
                dto.SubeSubeKodu
                    .Trim()
                    .ToUpperInvariant();

            if (subeKodu.Length > 20)
            {
                throw new ArgumentException(
                    "Şube kodu en fazla 20 karakter olabilir."
                );
            }

            if (
                subeKodu.Length != 5 ||
                subeKodu[0] != 'S' ||
                !subeKodu
                    .Substring(1)
                    .All(char.IsDigit)
            )
            {
                throw new ArgumentException(
                    "Şube kodu S ve ardından dört rakam içermelidir."
                );
            }


            // DÖVİZ CİNSİ

            if (
                dto.DovizCinsi ==
                DovizCinsiDurumlari.None
            )
            {
                throw new ArgumentException(
                    "Döviz cinsi seçilmelidir."
                );
            }

            if (
                !Enum.IsDefined(
                    typeof(DovizCinsiDurumlari),
                    dto.DovizCinsi
                )
            )
            {
                throw new ArgumentException(
                    "Geçersiz döviz cinsi."
                );
            }


            // HESAP TİPİ

            if (
                dto.HesapTipi ==
                HesapTipiDurumlari.None
            )
            {
                throw new ArgumentException(
                    "Hesap tipi seçilmelidir."
                );
            }

            if (
                !Enum.IsDefined(
                    typeof(HesapTipiDurumlari),
                    dto.HesapTipi
                )
            )
            {
                throw new ArgumentException(
                    "Geçersiz hesap tipi."
                );
            }


            // MÜŞTERİ ID

            if (dto.MusteriBilgileriId <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz müşteri ID bilgisi."
                );
            }


            // RECORD USER

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

                // HESAP GÜNCELLEME DOĞRULAMA METODU
        private void HesapGuncellemeRealityCheck(HesapDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Hesap bilgileri gönderilmelidir."
                );
            }

            // HESAP ADI

            if (string.IsNullOrWhiteSpace(dto.HesapAdi))
            {
                throw new ArgumentException(
                    "Hesap adı girilmesi zorunludur."
                );
            }

            if (dto.HesapAdi.Length > 50)
            {
                throw new ArgumentException(
                    "Hesap adı en fazla 50 karakter olabilir."
                );
            }

            // HESAP NUMARASI

            if (string.IsNullOrWhiteSpace(dto.HesapNo))
            {
                throw new ArgumentException(
                    "Hesap numarası girilmesi zorunludur."
                );
            }

            if (dto.HesapNo.Length != 16)
            {
                throw new ArgumentException(
                    "Hesap numarası 16 haneli olmalıdır."
                );
            }

            if (!dto.HesapNo.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "Hesap numarası yalnızca rakamlardan oluşmalıdır."
                );
            }

            // IBAN

            if (string.IsNullOrWhiteSpace(dto.IBAN))
            {
                throw new ArgumentException(
                    "IBAN girilmesi zorunludur."
                );
            }

            if (dto.IBAN.Length != 26)
            {
                throw new ArgumentException(
                    "IBAN 26 karakter olmalıdır."
                );
            }

            if (!dto.IBAN.StartsWith("TR", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "IBAN 'TR' ile başlamalıdır."
                );
            }

            string ibanSayisalKisim = dto.IBAN.Substring(2);

            if (!ibanSayisalKisim.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "IBAN, 'TR' ile başladıktan sonra gelen 24 karakter yalnızca rakamlardan oluşmalıdır."
                );
            }

            if (dto.IBAN.Substring(dto.IBAN.Length - 16) != dto.HesapNo)
            {
                throw new ArgumentException(
                    "IBAN'ın son 16 hanesi hesap numarası ile aynı olmalıdır."
                );
            }

            // BAKİYE

            if (dto.Bakiye < 0)
            {
                throw new ArgumentException(
                    "Bakiye negatif olamaz."
                );
            }

            // ŞUBE

            if (string.IsNullOrWhiteSpace(dto.SubeSubeKodu))
            {
                throw new ArgumentException(
                    "Şube seçilmesi zorunludur."
                );
            }

            if (dto.SubeSubeKodu.Length > 20)
            {
                throw new ArgumentException(
                    "Şube kodu en fazla 20 karakter olabilir."
                );
            }

            // DÖVİZ CİNSİ

            if (dto.DovizCinsi == DovizCinsiDurumlari.None)
            {
                throw new ArgumentException(
                    "Döviz cinsi seçilmelidir."
                );
            }

            if (!Enum.IsDefined(
                typeof(DovizCinsiDurumlari),
                dto.DovizCinsi))
            {
                throw new ArgumentException(
                    "Geçersiz döviz cinsi."
                );
            }

            // HESAP DURUMU

            if (dto.HesapDurumKodu == HesapDurumKodlari.None)
            {
                throw new ArgumentException(
                    "Hesap durumu seçilmelidir."
                );
            }

            if (!Enum.IsDefined(
                typeof(HesapDurumKodlari),
                dto.HesapDurumKodu))
            {
                throw new ArgumentException(
                    "Geçersiz hesap durumu."
                );
            }

            // HESAP TİPİ

            if (dto.HesapTipi == HesapTipiDurumlari.None)
            {
                throw new ArgumentException(
                    "Hesap tipi seçilmelidir."
                );
            }

            if (!Enum.IsDefined(
                typeof(HesapTipiDurumlari),
                dto.HesapTipi))
            {
                throw new ArgumentException(
                    "Geçersiz hesap tipi."
                );
            }

            // MÜŞTERİ ID

            if (dto.MusteriBilgileriId <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz müşteri ID'si."
                );
            }
        }

        public void IdCheck(long id)
        {
            if (id <= 0)
            {
                throw new Exception("Geçersiz ID");
            }
        }
    }
}

