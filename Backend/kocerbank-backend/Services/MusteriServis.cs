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

        // ORTAK DOĞRULAMA METODU
        private void RealityCheck(MusteriDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentException(
                    "Müşteri bilgileri gönderilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Ad))
            {
                throw new ArgumentException(
                    "Ad girilmesi zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(dto.Soyad))
            {
                throw new ArgumentException(
                    "Soyad girilmesi zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(dto.Eposta))
            {
                throw new ArgumentException(
                    "E-posta girilmesi zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
            {
                throw new ArgumentException(
                    "Telefon numarası girilmesi zorunludur.");
            }

            if (string.IsNullOrWhiteSpace(dto.SubeSubeKodu))
            {
                throw new ArgumentException(
                    "Şube seçilmesi zorunludur.");
            }

            if (dto.MusteriTipi ==
                MusteriTipiDurumlari.None)
            {
                throw new ArgumentException(
                    "Bireysel veya kurumsal müşteri tipi seçilmelidir.");
            }

            if (dto.DurumKodu ==
                AktifPasifDurumlari.None)
            {
                throw new ArgumentException(
                    "Durum seçilmelidir.");
            }

            if (dto.MusteriTipi ==
                MusteriTipiDurumlari.Bireysel)
            {
                if (string.IsNullOrWhiteSpace(dto.TCKN) ||
                    dto.TCKN.Length != 11)
                {
                    throw new ArgumentException(
                        "TCKN 11 haneli olmalıdır.");
                }

                if (!dto.DogumTarihi.HasValue && dto.MusteriTipi==MusteriTipiDurumlari.Bireysel)
                {
                    throw new ArgumentException(
                        "Doğum tarihi girilmelidir.");
                }

                if (!dto.Cinsiyet.HasValue ||
                    dto.Cinsiyet ==
                    CinsiyetDurumlari.None)
                {
                    throw new ArgumentException(
                        "Cinsiyet seçilmelidir.");
                }

                // Bireysel müşteride kullanılmaz.
                dto.VKN = null;
                dto.Unvan = null;
            }

            if (dto.MusteriTipi ==
                MusteriTipiDurumlari.Kurumsal)
            {
                if (string.IsNullOrWhiteSpace(dto.VKN) ||
                    dto.VKN.Length != 10)
                {
                    throw new ArgumentException(
                        "VKN 10 haneli olmalıdır.");
                }

                if (string.IsNullOrWhiteSpace(dto.Unvan))
                {
                    throw new ArgumentException(
                        "Unvan girilmesi zorunludur.");
                }

                // Kurumsal müşteride kullanılmaz.
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