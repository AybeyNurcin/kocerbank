using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class SubeService
    {
        private readonly SubeRepository _subeRepository;

        public SubeService(SubeRepository subeRepository)
        {
            _subeRepository = subeRepository;
        }

        // 1. ŞUBE EKLEME
        public SubeDTO Ekle(SubeDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentException(
                    "Şube bilgileri gönderilmelidir.");
            }

            SubeBilgileriniKontrolEt(dto);

            return _subeRepository.Ekle(dto);
        }

        // 2. ID'YE GÖRE ŞUBE GETİRME
        public SubeDTO? GetirById(long id)
        {
            IdKontrolEt(id);

            return _subeRepository.GetirById(id);
        }

        // 3. KRİTERE GÖRE ŞUBE LİSTELEME
        public List<SubeDTO> Listele(SubeDTO aramaKriterleri)
        {
            if (aramaKriterleri is null)
            {
                throw new ArgumentException(
                    "Arama kriterleri gönderilmelidir.");
            }

            if (!Enum.IsDefined(
                    typeof(AktifPasifDurumlari),
                    aramaKriterleri.SubeDurumKodu))
            {
                throw new ArgumentException(
                    "Geçersiz şube durum kodu gönderildi.");
            }

            return _subeRepository.GetirListele(aramaKriterleri);
        }

        // 4. ŞUBE GÜNCELLEME
        public void Guncelle(SubeDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentException(
                    "Güncellenecek şube bilgileri gönderilmelidir.");
            }

            IdKontrolEt(dto.Id);

            SubeBilgileriniKontrolEt(dto);

            SubeDTO? mevcutSube =
                _subeRepository.GetirById(dto.Id);

            if (mevcutSube is null)
            {
                throw new KeyNotFoundException(
                    "Güncellenecek şube bulunamadı.");
            }

            _subeRepository.Guncelle(dto);
        }

        // 5. ŞUBE SİLME
        public void Sil(long id)
        {
            IdKontrolEt(id);

            SubeDTO? mevcutSube =
                _subeRepository.GetirById(id);

            if (mevcutSube is null)
            {
                throw new KeyNotFoundException(
                    "Silinecek şube bulunamadı.");
            }

            _subeRepository.Sil(id);
        }

        // EKLEME VE GÜNCELLEMEDE ORTAK KONTROLLER
        private static void SubeBilgileriniKontrolEt(
            SubeDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SubeAdi))
            {
                throw new ArgumentException(
                    "Şube adı boş bırakılamaz.");
            }

            if (dto.SubeAdi.Length > 50)
            {
                throw new ArgumentException(
                    "Şube adı en fazla 50 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.SubeTelefonNo))
            {
                throw new ArgumentException(
                    "Şube telefon numarası boş bırakılamaz.");
            }

            if (dto.SubeTelefonNo.Length != 11)
            {
                throw new ArgumentException(
                    "Şube telefon numarası 11 karakter olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(dto.SubeAdres))
            {
                throw new ArgumentException(
                    "Şube adresi boş bırakılamaz.");
            }

            if (dto.SubeAdres.Length > 50)
            {
                throw new ArgumentException(
                    "Şube adresi en fazla 50 karakter olabilir.");
            }

            if (dto.SubeDurumKodu ==
                AktifPasifDurumlari.None)
            {
                throw new ArgumentException(
                    "Şube durumu Aktif veya Pasif olmalıdır.");
            }

            if (!Enum.IsDefined(
                    typeof(AktifPasifDurumlari),
                    dto.SubeDurumKodu))
            {
                throw new ArgumentException(
                    "Geçersiz şube durum kodu gönderildi.");
            }

            if (!string.IsNullOrWhiteSpace(dto.RecordUser) &&
                dto.RecordUser.Length > 10)
            {
                throw new ArgumentException(
                    "RecordUser en fazla 10 karakter olabilir.");
            }
        }

        // ID KULLANAN METOTLARIN ORTAK KONTROLÜ
        private static void IdKontrolEt(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Şube ID değeri sıfırdan büyük olmalıdır.");
            }
        }
    }
}