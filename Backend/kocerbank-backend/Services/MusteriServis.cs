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
        public MusteriDTO? GetirById(long id)
        {
            var musteri = _musteriRepository.GetirById(id);
            if (musteri == null)
            {
                throw new KeyNotFoundException($"Müşteri bulunamadı: ID = {id}");
            }

            if (id <= 0)
            {
                throw new Exception("Geçersiz müşteri ID'si.");
            }

            return _musteriRepository.GetirById(id);
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
            if (string.IsNullOrWhiteSpace(dto.Ad))
                throw new Exception("Ad girilmesi zorunludur.");

            if (string.IsNullOrWhiteSpace(dto.Soyad))
                throw new Exception("Soyad girilmesi zorunludur.");

            if (string.IsNullOrWhiteSpace(dto.Eposta))
                throw new Exception("E-posta girilmesi zorunludur.");

            if (string.IsNullOrWhiteSpace(dto.DogumTarihi.ToString()))
                throw new Exception("Doğum tarihi boş bırakılamaz.");

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
                throw new Exception("Telefon numarası girilmesi zorunludur.");

            if (string.IsNullOrWhiteSpace(dto.SubeSubeKodu))
                throw new Exception("Şube kodu seçilmesi zorunludur.");

            if (dto.MusteriTipi == MusteriTipiDurumlari.None)
                throw new Exception("Bireysel/Kurumsal seçilmelidir.");

            if (dto.MusteriTipi == MusteriTipiDurumlari.Bireysel && dto.TCKN.Length != 11)
                throw new Exception("TCKN 11 haneli olmalıdır.");

            if (dto.MusteriTipi == MusteriTipiDurumlari.Kurumsal && dto.VKN.Length != 10)
                throw new Exception("VKN 10 haneli olmalıdır.");

            if (dto.MusteriTipi == MusteriTipiDurumlari.Bireysel && dto.Cinsiyet == CinsiyetDurumlari.None)
                throw new Exception("Cinsiyet seçilmelidir.");

            if (dto.DurumKodu == AktifPasifDurumlari.None)
                throw new Exception("Durum seçilmelidir.");
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