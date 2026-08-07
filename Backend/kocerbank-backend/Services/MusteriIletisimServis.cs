using kocerbank_backend.DataAccess;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class MusteriIletisimService
    {
        private readonly MusteriIletisimRepository
            _musteriIletisimRepository;

        private readonly AktifPersonelServis
            _aktifPersonelServis;

        public MusteriIletisimService(
            MusteriIletisimRepository musteriIletisimRepository,
            AktifPersonelServis aktifPersonelServis)
        {
            _musteriIletisimRepository =
                musteriIletisimRepository;

            _aktifPersonelServis =
                aktifPersonelServis;
        }

        // 1. İLETİŞİM BİLGİSİ EKLEME
        public MusteriIletisimDTO Ekle(
            MusteriIletisimDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "İletişim bilgileri gönderilmelidir.");
            }

            if (dto.MusteriBilgileriId <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz müşteri ID'si.");
            }

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
            {
                throw new ArgumentException(
                    "Cep telefonu girilmesi zorunludur.");
            }

            dto.TelefonNo =
                dto.TelefonNo.Trim();

            if (dto.TelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "Cep telefonu en fazla 13 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Eposta))
            {
                throw new ArgumentException(
                    "E-posta girilmesi zorunludur.");
            }

            dto.Eposta =
                dto.Eposta.Trim();

            if (dto.Eposta.Length > 50)
            {
                throw new ArgumentException(
                    "E-posta en fazla 50 karakter olabilir.");
            }

            IletisimAlanlariniTemizleVeKontrolEt(dto);

            // Frontend'den gelen RecordUser kullanılmaz.
            // Giriş yapan personelin sicili backend'den alınır.
            dto.RecordUser =
                _aktifPersonelServis.SicilNoGetir();

            return _musteriIletisimRepository.Ekle(dto);
        }

        // 2. MÜŞTERİ ID'SİNE GÖRE
        // İLETİŞİM BİLGİSİ GETİRME
        public MusteriIletisimDTO GetirById(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz müşteri ID'si.");
            }

            MusteriIletisimDTO? iletisim =
                _musteriIletisimRepository.GetirById(id);

            if (iletisim is null)
            {
                throw new KeyNotFoundException(
                    $"İletişim bilgisi bulunamadı: ID = {id}");
            }

            return iletisim;
        }

        // 3. İLETİŞİM BİLGİLERİNİ GÜNCELLEME
        public void Guncelle(
            MusteriIletisimAramaKriterleriDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "İletişim bilgileri gönderilmelidir.");
            }

            if (dto.MusteriBilgileriId <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz müşteri ID'si.");
            }

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
            {
                throw new ArgumentException(
                    "Cep telefonu girilmesi zorunludur.");
            }

            dto.TelefonNo =
                dto.TelefonNo.Trim();

            if (dto.TelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "Cep telefonu en fazla 13 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Eposta))
            {
                throw new ArgumentException(
                    "E-posta girilmesi zorunludur.");
            }

            dto.Eposta =
                dto.Eposta.Trim();

            if (dto.Eposta.Length > 50)
            {
                throw new ArgumentException(
                    "E-posta en fazla 50 karakter olabilir.");
            }

            IletisimAlanlariniTemizleVeKontrolEt(dto);

            MusteriIletisimDTO? mevcutIletisim =
                _musteriIletisimRepository.GetirById(
                    dto.MusteriBilgileriId);

            if (mevcutIletisim is null)
            {
                throw new KeyNotFoundException(
                    $"{dto.MusteriBilgileriId} ID'li iletişim bilgisi bulunamadı.");
            }

            // Güncellemeyi yapan personelin sicili
            // frontend yerine backend'den alınır.
            dto.RecordUser =
                _aktifPersonelServis.SicilNoGetir();

            _musteriIletisimRepository.Guncelle(dto);
        }

        // EKLEMEDEKİ OPSİYONEL ALANLARIN
        // TEMİZLENMESİ VE KONTROLÜ
        private static void IletisimAlanlariniTemizleVeKontrolEt(
            MusteriIletisimDTO dto)
        {
            dto.EvTelefonNo =
                MetniTemizle(dto.EvTelefonNo);

            dto.IsTelefonNo =
                MetniTemizle(dto.IsTelefonNo);

            dto.EvAdres =
                MetniTemizle(dto.EvAdres);

            dto.IsAdres =
                MetniTemizle(dto.IsAdres);

            OpsiyonelAlanlariKontrolEt(
                dto.EvTelefonNo,
                dto.IsTelefonNo,
                dto.EvAdres,
                dto.IsAdres);
        }

        // GÜNCELLEMEDEKİ OPSİYONEL ALANLARIN
        // TEMİZLENMESİ VE KONTROLÜ
        private static void IletisimAlanlariniTemizleVeKontrolEt(
            MusteriIletisimAramaKriterleriDTO dto)
        {
            dto.EvTelefonNo =
                MetniTemizle(dto.EvTelefonNo);

            dto.IsTelefonNo =
                MetniTemizle(dto.IsTelefonNo);

            dto.EvAdres =
                MetniTemizle(dto.EvAdres);

            dto.IsAdres =
                MetniTemizle(dto.IsAdres);

            OpsiyonelAlanlariKontrolEt(
                dto.EvTelefonNo,
                dto.IsTelefonNo,
                dto.EvAdres,
                dto.IsAdres);
        }

        private static void OpsiyonelAlanlariKontrolEt(
            string? evTelefonNo,
            string? isTelefonNo,
            string? evAdres,
            string? isAdres)
        {
            if (evTelefonNo is not null &&
                evTelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "Ev telefonu en fazla 13 karakter olabilir.");
            }

            if (isTelefonNo is not null &&
                isTelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "İş telefonu en fazla 13 karakter olabilir.");
            }

            if (evAdres is not null &&
                evAdres.Length > 100)
            {
                throw new ArgumentException(
                    "Ev adresi en fazla 100 karakter olabilir.");
            }

            if (isAdres is not null &&
                isAdres.Length > 100)
            {
                throw new ArgumentException(
                    "İş adresi en fazla 100 karakter olabilir.");
            }
        }

        private static string? MetniTemizle(
            string? metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
            {
                return null;
            }

            return metin.Trim();
        }
    }
}