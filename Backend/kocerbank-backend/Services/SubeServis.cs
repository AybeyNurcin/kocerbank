using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class SubeService
    {
        private readonly SubeRepository _subeRepository;
        private readonly AktifPersonelServis _aktifPersonelServis;

        public SubeService(
            SubeRepository subeRepository,
            AktifPersonelServis aktifPersonelServis)
        {
            _subeRepository = subeRepository;
            _aktifPersonelServis = aktifPersonelServis;
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

            SubeAramaKriterleriDTO aramaKriterleri =
                new SubeAramaKriterleriDTO
                {
                    SubeAdi = dto.SubeAdi,
                    SubeTelefonNo = dto.SubeTelefonNo,
                    SubeAdres = dto.SubeAdres
                };

            List<SubeDTO> bulunanSubeler =
                Listele(aramaKriterleri);

            if (bulunanSubeler.Count > 0)
            {
                throw new InvalidOperationException(
                    "Girilen bilgilere sahip bir şube zaten bulunmaktadır.");
            }

            // Frontend'den gelen RecordUser dikkate alınmaz.
            // Giriş yapan personelin sicili backend tarafından atanır.
            dto.RecordUser =
                _aktifPersonelServis.SicilNoGetir();

            return _subeRepository.Ekle(dto);
        }

        // 2. ID'YE GÖRE ŞUBE GETİRME
        public SubeDTO GetirById(long id)
        {
            IdKontrolEt(id);

            SubeDTO? sube =
                _subeRepository.GetirById(id);

            if (sube is null)
            {
                throw new KeyNotFoundException(
                    "Bu ID değerine ait şube bulunamadı.");
            }

            return sube;
        }

        // 3. KRİTERE GÖRE ŞUBE LİSTELEME
        public List<SubeDTO> Listele(
            SubeAramaKriterleriDTO aramaKriterleri)
        {
            return _subeRepository.GetirListele(
                aramaKriterleri);
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

            SubeAramaKriterleriDTO aramaKriterleri =
                new SubeAramaKriterleriDTO
                {
                    SubeAdi = dto.SubeAdi,
                    SubeTelefonNo = dto.SubeTelefonNo,
                    SubeAdres = dto.SubeAdres
                };

            List<SubeDTO> bulunanSubeler =
                Listele(aramaKriterleri);

            bool baskaSubeVarMi =
                bulunanSubeler.Any(
                    sube => sube.Id != dto.Id);

            if (baskaSubeVarMi)
            {
                throw new InvalidOperationException(
                    "Girilen bilgilere sahip başka bir şube zaten bulunmaktadır.");
            }

            dto.RecordUser =
                _aktifPersonelServis.SicilNoGetir();

            _subeRepository.Guncelle(dto);
        }

        // 5. ŞUBE SİLME
        public void Sil(long id)
        {
            _ = GetirById(id);

            _subeRepository.Sil(id);
        }

        public SubeDashboardDTO GetirDashboardOzet()
        {
            return _subeRepository.GetirDashboardOzet();
        }

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
                    "Şube telefon numarası 11 haneli olmalıdır.");
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
        }

        private static void IdKontrolEt(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Bu ID değerinde şube olamaz.");
            }
        }
    }
}