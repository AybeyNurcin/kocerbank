using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class MusteriService
    {
        private readonly MusteriRepository _musteriRepository;

        public MusteriService(MusteriRepository musteriRepository)
        {
            _musteriRepository = musteriRepository;
        }

        // 1. EKLEME
        public MusteriDTO Ekle(MusteriDTO dto)
        {
            RealityCheck(dto);

            return _musteriRepository.Ekle(dto);
        }

        // 2. ID'YE GÖRE GETİR
        public MusteriDTO GetirById(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz müşteri ID'si.");
            }

            MusteriDTO? musteri =
                _musteriRepository.GetirById(id);

            if (musteri is null)
            {
                throw new KeyNotFoundException(
                    $"Müşteri bulunamadı: ID = {id}");
            }

            return musteri;
        }

        // 3. LİSTELEME
        public List<MusteriDTO> Listele(MusteriAramaKriterleriDTO aramaKriterleri)
        {
            // Burada zorunlu doğrulama yok çünkü kriterler opsiyonel (None/boş olabilir)
            return _musteriRepository.Listele(aramaKriterleri);
        }

        // 4. GÜNCELLEME
        public void Guncelle(MusteriDTO dto)
        {
            RealityCheck(dto);

            MusteriDTO? mevcutMusteri = _musteriRepository.GetirById(dto.Id);

            if (mevcutMusteri is null)
            {
                throw new KeyNotFoundException(
                    $"{dto.Id} ID'li müşteri bulunamadı.");
            }

            _musteriRepository.Guncelle(dto);
        }

        // 5. SİLME
        public void Sil(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Geçersiz müşteri ID'si.");
            }

            MusteriDTO? mevcutMusteri = _musteriRepository.GetirById(id);

            if (mevcutMusteri is null)
            {
                throw new KeyNotFoundException(
                    $"{id} ID'li müşteri bulunamadı.");
            }

            _musteriRepository.Sil(id);
        }

        public MusteriDashboardDTO GetirDashboardOzet()
        {
            return _musteriRepository.GetirDashboardOzet();
        }

        // ORTAK DOĞRULAMA METODU
        private void RealityCheck(MusteriDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Müşteri bilgileri gönderilmelidir."
                );
            }

            // ORTAK ALANLAR

            if (string.IsNullOrWhiteSpace(dto.Ad))
            {
                throw new ArgumentException(
                    "Ad girilmesi zorunludur."
                );
            }

            if (dto.Ad.Length > 50)
            {
                throw new ArgumentException(
                    "Ad en fazla 50 karakter olabilir."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.Soyad))
            {
                throw new ArgumentException(
                    "Soyad girilmesi zorunludur."
                );
            }

            if (dto.Soyad.Length > 50)
            {
                throw new ArgumentException(
                    "Soyad en fazla 50 karakter olabilir."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.Eposta))
            {
                throw new ArgumentException(
                    "E-posta girilmesi zorunludur."
                );
            }

            if (dto.Eposta.Length > 50)
            {
                throw new ArgumentException(
                    "E-posta en fazla 50 karakter olabilir."
                );
            }

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
            {
                throw new ArgumentException(
                    "Telefon numarası girilmesi zorunludur."
                );
            }

            if (dto.TelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "Telefon numarası en fazla 13 karakter olabilir."
                );
            }

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

            // MÜŞTERİ TİPİ KONTROLÜ

            if (
                dto.MusteriTipi ==
                MusteriTipiDurumlari.None
            )
            {
                throw new ArgumentException(
                    "Müşteri tipi seçilmelidir."
                );
            }

            if (
                !Enum.IsDefined(
                    typeof(MusteriTipiDurumlari),
                    dto.MusteriTipi
                )
            )
            {
                throw new ArgumentException(
                    "Geçersiz müşteri tipi."
                );
            }

            // DURUM KONTROLÜ

            if (
                dto.DurumKodu ==
                AktifPasifDurumlari.None
            )
            {
                throw new ArgumentException(
                    "Aktif veya pasif durumu seçilmelidir."
                );
            }

            if (
                !Enum.IsDefined(
                    typeof(AktifPasifDurumlari),
                    dto.DurumKodu
                )
            )
            {
                throw new ArgumentException(
                    "Geçersiz müşteri durum kodu."
                );
            }

            // BİREYSEL MÜŞTERİ KONTROLLERİ

            if (
                dto.MusteriTipi ==
                MusteriTipiDurumlari.Bireysel
            )
            {
                if (dto.DogumTarihi is null)
                {
                    throw new ArgumentException(
                        "Bireysel müşteriler için doğum tarihi zorunludur."
                    );
                }

                if (string.IsNullOrWhiteSpace(dto.TCKN))
                {
                    throw new ArgumentException(
                        "Bireysel müşteriler için TCKN zorunludur."
                    );
                }

                if (
                    dto.TCKN.Length != 11 ||
                    !dto.TCKN.All(char.IsDigit)
                )
                {
                    throw new ArgumentException(
                        "TCKN 11 haneli ve yalnızca rakamlardan oluşmalıdır."
                    );
                }

                if (
                    dto.Cinsiyet is null ||
                    dto.Cinsiyet ==
                    CinsiyetDurumlari.None
                )
                {
                    throw new ArgumentException(
                        "Bireysel müşteriler için cinsiyet seçilmelidir."
                    );
                }

                if (
                    !Enum.IsDefined(
                        typeof(CinsiyetDurumlari),
                        dto.Cinsiyet.Value
                    )
                )
                {
                    throw new ArgumentException(
                        "Geçersiz cinsiyet değeri."
                    );
                }

                // Bireysel müşteride kurumsal alanlar kullanılmaz.
                dto.VKN = null;
                dto.Unvan = null;
            }

            // KURUMSAL MÜŞTERİ KONTROLLERİ

            if (
                dto.MusteriTipi ==
                MusteriTipiDurumlari.Kurumsal
            )
            {
                if (string.IsNullOrWhiteSpace(dto.VKN))
                {
                    throw new ArgumentException(
                        "Kurumsal müşteriler için VKN zorunludur."
                    );
                }

                if (
                    dto.VKN.Length != 10 ||
                    !dto.VKN.All(char.IsDigit)
                )
                {
                    throw new ArgumentException(
                        "VKN 10 haneli ve yalnızca rakamlardan oluşmalıdır."
                    );
                }

                if (string.IsNullOrWhiteSpace(dto.Unvan))
                {
                    throw new ArgumentException(
                        "Kurumsal müşteriler için unvan zorunludur."
                    );
                }

                if (dto.Unvan.Length > 50)
                {
                    throw new ArgumentException(
                        "Unvan en fazla 50 karakter olabilir."
                    );
                }

                // Kurumsal müşteride bireysel alanlar kullanılmaz.
                dto.TCKN = null;
                dto.DogumTarihi = null;
                dto.Cinsiyet = null;
            }
        }

        public void idCheck(long id)
        {
            if (id <= 0)
            {
                throw new Exception("Geçersiz ID");
            }
        }
    }
}