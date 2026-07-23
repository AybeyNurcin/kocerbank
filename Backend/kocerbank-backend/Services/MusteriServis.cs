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
            Dogrula(dto);

            // Örn: Şifreyi düz metin olarak kaydetmemek için hash'leme burada yapılır
            // dto.Sifre = _hashService.Hashle(dto.Sifre);

            dto.KayitOlusturmaTarihi = DateTime.Now;

            return _musteriRepository.Ekle(dto);
        }

        // 2. ID'YE GÖRE GETİR
        public MusteriDTO? GetirById(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Geçersiz müşteri ID'si.");
            }

            return _musteriRepository.GetirById(id);
        }

        // 3. LİSTELEME
        public List<MusteriDTO> Listele(MusteriDTO aramaKriterleri)
        {
            // Burada zorunlu doğrulama yok çünkü kriterler opsiyonel (None/boş olabilir)
            return _musteriRepository.Listele(aramaKriterleri);
        }

        // 4. GÜNCELLEME
        public void Guncelle(MusteriDTO dto)
        {
            Dogrula(dto);

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
        private void Dogrula(MusteriDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Ad))
                throw new ArgumentException("Ad alanı boş olamaz.");

            if (string.IsNullOrWhiteSpace(dto.Soyad))
                throw new ArgumentException("Soyad alanı boş olamaz.");

            if (string.IsNullOrWhiteSpace(dto.Eposta))
                throw new ArgumentException("E-posta alanı boş olamaz.");

            if (dto.MusteriTipi == MusteriTipiDurumlari.None)
                throw new ArgumentException("Müşteri tipi seçilmelidir.");

            // İş kuralı: Bireysel müşteride TCKN, Kurumsal'da VKN zorunlu
            /*if (dto.MusteriTipi == MusteriTipiDurumlari.Bireysel && dto.TCKN.ToString().Length != 11)
                throw new ArgumentException("Bireysel müşteri için TCKN 11 haneli olmalıdır.");

            if (dto.MusteriTipi == MusteriTipiDurumlari.Kurumsal && dto.VKN.ToString().Length != 10)
                throw new ArgumentException("Kurumsal müşteri için VKN 10 haneli olmalıdır.");*/

            if (dto.DurumKodu == AktifPasifDurumlari.None)
                throw new ArgumentException("Durum kodu seçilmelidir.");
        }
    }
}