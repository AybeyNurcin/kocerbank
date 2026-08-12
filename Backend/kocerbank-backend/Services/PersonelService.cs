using System.Net.Mail;
using System.Text.RegularExpressions;
using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace kocerbank_backend.Services
{
    public class PersonelService : BaseCrudService
    {
        private readonly PersonelRepository _personelRepository;

        private readonly PasswordHasher<PersonelDTO>
            _passwordHasher;

        public PersonelService(
            PersonelRepository personelRepository,
            AktifPersonelService aktifPersonelServis)
            : base(aktifPersonelServis)
        {
            _personelRepository = personelRepository;

            _passwordHasher =
                new PasswordHasher<PersonelDTO>();
        }

        // 1. PERSONEL EKLEME
        public PersonelDTO Ekle(PersonelDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentException(
                    "Personel bilgileri gönderilmelidir.");
            }

            PersonelBilgileriniDuzenle(dto);
            PersonelBilgileriniKontrolEt(dto);

            if (string.IsNullOrWhiteSpace(dto.Sifre))
            {
                throw new ArgumentException(
                    "Şifre boş bırakılamaz.");
            }

            if (dto.Sifre.Length < 8)
            {
                throw new ArgumentException(
                    "Şifre en az 8 karakter olmalıdır.");
            }

            PersonelAramaKriterleriDTO kriter =
                new PersonelAramaKriterleriDTO
                {
                    TCKN = dto.TCKN
                };

            bool tcknKullaniliyorMu =
                Listele(kriter).Count > 0;

            if (tcknKullaniliyorMu)
            {
                throw new InvalidOperationException(
                    "Bu TC Kimlik Numarasına sahip bir personel zaten bulunmaktadır.");
            }

            dto.Sifre =
                _passwordHasher.HashPassword(
                    dto,
                    dto.Sifre);

            // Frontend'den gelen RecordUser kullanılmaz.
            // Giriş yapan personelin sicili backend tarafından atanır.
            dto.RecordUser =
                GirisYapanPersonelSicili();

            return _personelRepository.Ekle(dto);
        }

        // 2. ID'YE GÖRE PERSONEL GETİRME
        public PersonelDTO GetirById(long id)
        {
            IdKontrolEt(id, "Personel ID değeri sıfırdan büyük olmalıdır.");

            return KaydiBulunduMuKontrolEt(
                _personelRepository.GetirById(id),
                $"Personel bulunamadı: ID = {id}");
        }

        // 3. PERSONEL GİRİŞİ
        public PersonelDTO Login(
            PersonelLoginDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentException(
                    "Giriş bilgileri gönderilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Sicil))
            {
                throw new ArgumentException(
                    "Sicil boş bırakılamaz.");
            }

            if (string.IsNullOrWhiteSpace(dto.Sifre))
            {
                throw new ArgumentException(
                    "Şifre boş bırakılamaz.");
            }

            string duzenlenmisSicil =
                dto.Sicil
                    .Trim()
                    .ToUpperInvariant();

            PersonelDTO? personel =
                GetirBySicil(duzenlenmisSicil);

            if (personel is null)
            {
                throw new UnauthorizedAccessException(
                    "Sicil veya şifre hatalı.");
            }

            PasswordVerificationResult sonuc =
                _passwordHasher.VerifyHashedPassword(
                    personel,
                    personel.Sifre,
                    dto.Sifre);

            if (sonuc ==
                PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Sicil veya şifre hatalı.");
            }

            if (personel.DurumKodu !=
                AktifPasifDurumlari.Aktif)
            {
                throw new UnauthorizedAccessException(
                    "Personel hesabı aktif değildir.");
            }

            return personel;
        }

        // SİCİLE GÖRE PERSONEL GETİRME
        private PersonelDTO? GetirBySicil(
            string sicil)
        {
            if (string.IsNullOrWhiteSpace(sicil))
            {
                throw new ArgumentException(
                    "Sicil boş bırakılamaz.");
            }

            string duzenlenmisSicil =
                sicil
                    .Trim()
                    .ToUpperInvariant();

            return _personelRepository.GetirBySicil(
                duzenlenmisSicil);
        }

        // 4. KRİTERE GÖRE PERSONEL LİSTELEME
        public List<PersonelDTO> Listele(
            PersonelAramaKriterleriDTO? aramaKriterleri)
        {
            aramaKriterleri ??=
                new PersonelAramaKriterleriDTO();

            PersonelAramaKriterleriniDuzenle(
                aramaKriterleri);

            return _personelRepository.GetirListele(
                aramaKriterleri);
        }

        // 5. PERSONEL GÜNCELLEME
        public void Guncelle(PersonelDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentException(
                    "Güncellenecek personel bilgileri gönderilmelidir.");
            }

            IdKontrolEt(dto.Id, "Personel ID değeri sıfırdan büyük olmalıdır.");

            PersonelBilgileriniDuzenle(dto);
            PersonelBilgileriniKontrolEt(dto);

            PersonelDTO mevcutPersonel =
                KaydiBulunduMuKontrolEt(
                    _personelRepository.GetirById(dto.Id),
                    "Güncellenecek personel bulunamadı.");

            PersonelAramaKriterleriDTO kriter =
                new PersonelAramaKriterleriDTO
                {
                    TCKN = dto.TCKN
                };

            bool tcknBaskaPersoneldeVarMi =
                Listele(kriter).Any(
                    personel =>
                        personel.Id != dto.Id);

            if (tcknBaskaPersoneldeVarMi)
            {
                throw new InvalidOperationException(
                    "Bu TC Kimlik Numarası başka bir personel tarafından kullanılmaktadır.");
            }

            // Normal personel güncellemesinde şifre değiştirilemez.
            // Database'deki mevcut hash korunur.
            dto.Sifre = mevcutPersonel.Sifre;

            // Frontend'den gelen RecordUser kullanılmaz.
            dto.RecordUser =
                GirisYapanPersonelSicili();

            _personelRepository.Guncelle(dto);
        }

        // 6. PERSONEL ŞİFRE DEĞİŞTİRME
        public void SifreDegistir(
            long id,
            PersonelSifreDegistirDTO dto)
        {
            IdKontrolEt(id, "Personel ID değeri sıfırdan büyük olmalıdır.");

            if (dto is null)
            {
                throw new ArgumentException(
                    "Şifre değiştirme bilgileri gönderilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.EskiSifre))
            {
                throw new ArgumentException(
                    "Eski şifre boş bırakılamaz.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.YeniSifre))
            {
                throw new ArgumentException(
                    "Yeni şifre boş bırakılamaz.");
            }

            if (dto.YeniSifre.Length < 8)
            {
                throw new ArgumentException(
                    "Yeni şifre en az 8 karakter olmalıdır.");
            }

            PersonelDTO mevcutPersonel =
                KaydiBulunduMuKontrolEt(
                    _personelRepository.GetirById(id),
                    "Personel bulunamadı.");

            PasswordVerificationResult sonuc =
                _passwordHasher.VerifyHashedPassword(
                    mevcutPersonel,
                    mevcutPersonel.Sifre,
                    dto.EskiSifre);

            if (sonuc ==
                PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException(
                    "Eski şifre hatalı.");
            }

            mevcutPersonel.Sifre =
                _passwordHasher.HashPassword(
                    mevcutPersonel,
                    dto.YeniSifre);

            // Şifre değişikliği de bir güncellemedir.
            mevcutPersonel.RecordUser =
                GirisYapanPersonelSicili();

            _personelRepository.Guncelle(
                mevcutPersonel);
        }

        // 7. PERSONEL SİLME
        public void Sil(long id)
        {
            IdKontrolEt(id, "Personel ID değeri sıfırdan büyük olmalıdır.");

            _ = KaydiBulunduMuKontrolEt(
                _personelRepository.GetirById(id),
                "Silinecek personel bulunamadı.");

            _personelRepository.Sil(id);
        }

        // 8. PERSONEL DASHBOARD ÖZETİ
        public PersonelDashboardDTO GetirDashboardOzet(
            DashboardFiltreDTO? filtre)
        {
            DateTime? baslangicTarihi =
                filtre?.BaslangicTarihi;

            DateTime? bitisTarihi =
                filtre?.BitisTarihi;

            if (baslangicTarihi.HasValue &&
                bitisTarihi.HasValue &&
                baslangicTarihi.Value.Date >
                bitisTarihi.Value.Date)
            {
                throw new ArgumentException(
                    "Başlangıç tarihi bitiş tarihinden sonra olamaz.");
            }

            return _personelRepository
                .GetirDashboardOzet(
                    baslangicTarihi,
                    bitisTarihi);
        }

        // PERSONEL BİLGİLERİNİ DÜZENLEME
        private static void PersonelBilgileriniDuzenle(
            PersonelDTO dto)
        {
            dto.Ad =
                (dto.Ad ?? string.Empty).Trim();

            dto.Soyad =
                (dto.Soyad ?? string.Empty).Trim();

            dto.Rol =
                (dto.Rol ?? string.Empty).Trim();

            dto.TCKN =
                (dto.TCKN ?? string.Empty).Trim();

            dto.TelefonNo =
                (dto.TelefonNo ?? string.Empty).Trim();

            dto.Adres =
                (dto.Adres ?? string.Empty).Trim();

            dto.Email =
                (dto.Email ?? string.Empty)
                    .Trim()
                    .ToLowerInvariant();

            dto.SubeKodu =
                (dto.SubeKodu ?? string.Empty)
                    .Trim()
                    .ToUpperInvariant();
        }

        // PERSONEL ARAMA KRİTERLERİNİ DÜZENLEME
        private static void
            PersonelAramaKriterleriniDuzenle(
                PersonelAramaKriterleriDTO kriter)
        {
            kriter.Ad =
                BosIseNull(kriter.Ad);

            kriter.Soyad =
                BosIseNull(kriter.Soyad);

            kriter.Rol =
                BosIseNull(kriter.Rol);

            kriter.Sicil =
                BosIseNull(kriter.Sicil)?
                    .ToUpperInvariant();

            kriter.TCKN =
                BosIseNull(kriter.TCKN);

            kriter.TelefonNo =
                BosIseNull(kriter.TelefonNo);

            kriter.Adres =
                BosIseNull(kriter.Adres);

            kriter.Email =
                BosIseNull(kriter.Email)?
                    .ToLowerInvariant();

            kriter.SubeKodu =
                BosIseNull(kriter.SubeKodu)?
                    .ToUpperInvariant();
        }

        private static string? BosIseNull(
            string? deger)
        {
            return string.IsNullOrWhiteSpace(deger)
                ? null
                : deger.Trim();
        }

        // EKLEME VE GÜNCELLEMEDE ORTAK KONTROLLER
        private static void PersonelBilgileriniKontrolEt(
            PersonelDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Ad))
            {
                throw new ArgumentException(
                    "Personel adı boş bırakılamaz.");
            }

            if (dto.Ad.Length > 50)
            {
                throw new ArgumentException(
                    "Personel adı en fazla 50 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Soyad))
            {
                throw new ArgumentException(
                    "Personel soyadı boş bırakılamaz.");
            }

            if (dto.Soyad.Length > 50)
            {
                throw new ArgumentException(
                    "Personel soyadı en fazla 50 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Rol))
            {
                throw new ArgumentException(
                    "Personel rolü boş bırakılamaz.");
            }

            if (dto.Rol.Length > 20)
            {
                throw new ArgumentException(
                    "Personel rolü en fazla 20 karakter olabilir.");
            }

            if (!Regex.IsMatch(
                    dto.TCKN,
                    @"^[0-9]{11}$"))
            {
                throw new ArgumentException(
                    "Personel TC Kimlik No 11 haneli ve yalnızca rakamlardan oluşmalıdır.");
            }

            if (!Regex.IsMatch(
                    dto.TelefonNo,
                    @"^[0-9]{11}$"))
            {
                throw new ArgumentException(
                    "Personel telefon numarası 11 haneli ve yalnızca rakamlardan oluşmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(dto.Adres))
            {
                throw new ArgumentException(
                    "Personel adresi boş bırakılamaz.");
            }

            if (dto.Adres.Length > 50)
            {
                throw new ArgumentException(
                    "Personel adresi en fazla 50 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException(
                    "Personel e-posta adresi boş bırakılamaz.");
            }

            if (dto.Email.Length > 50)
            {
                throw new ArgumentException(
                    "Personel e-posta adresi en fazla 50 karakter olabilir.");
            }

            if (!MailAddress.TryCreate(
                    dto.Email,
                    out _))
            {
                throw new ArgumentException(
                    "Geçerli bir e-posta adresi girilmelidir.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.SubeKodu))
            {
                throw new ArgumentException(
                    "Personelin bağlı olduğu şube seçilmelidir.");
            }

            if (dto.SubeKodu.Length > 20)
            {
                throw new ArgumentException(
                    "Şube kodu en fazla 20 karakter olabilir.");
            }

            if (!Enum.IsDefined(
                    typeof(AktifPasifDurumlari),
                    dto.DurumKodu) ||
                dto.DurumKodu ==
                    AktifPasifDurumlari.None)
            {
                throw new ArgumentException(
                    "Personel durumu Aktif veya Pasif olmalıdır.");
            }
        }

    }
}