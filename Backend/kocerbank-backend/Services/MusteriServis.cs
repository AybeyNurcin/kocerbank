using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class MusteriService
    {
        private readonly MusteriRepository
            _musteriRepository;

        private readonly AktifPersonelServis
            _aktifPersonelServis;

        public MusteriService(
            MusteriRepository musteriRepository,
            AktifPersonelServis aktifPersonelServis)
        {
            _musteriRepository =
                musteriRepository;

            _aktifPersonelServis =
                aktifPersonelServis;
        }

        // 1. MÜŞTERİ EKLEME
        public MusteriDTO Ekle(MusteriDTO dto)
        {
            RealityCheck(dto);

            // Frontend'den gelen RecordUser kullanılmaz.
            // Giriş yapan personelin sicili backend'de atanır.
            dto.RecordUser =
                _aktifPersonelServis.SicilNoGetir();

            return _musteriRepository.Ekle(dto);
        }

        // MÜŞTERİ VE İLETİŞİM BİLGİLERİNİ
        // BİRLİKTE EKLEME
        public MusteriTamKaydetSonucDTO TamKaydet(
            MusteriTamKaydetDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Müşteri bilgileri gönderilmelidir.");
            }

            if (dto.Musteri is null)
            {
                throw new ArgumentException(
                    "Müşteri bilgileri gönderilmelidir.");
            }

            if (dto.Iletisim is null)
            {
                throw new ArgumentException(
                    "İletişim bilgileri gönderilmelidir.");
            }

            // Mevcut müşteri kontrollerini kullanır.
            RealityCheck(dto.Musteri);

            // İkinci adımdaki iletişim
            // alanlarını kontrol eder.
            IletisimBilgileriniKontrolEt(
                dto.Iletisim);

            // Müşteri ve iletişim kayıtlarının ikisine de
            // aktarılacak giriş yapan personel sicili.
            dto.Musteri.RecordUser =
                _aktifPersonelServis.SicilNoGetir();

            return _musteriRepository.TamKaydet(dto);
        }

        // 2. ID'YE GÖRE MÜŞTERİ GETİRME
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

        // 3. KRİTERE GÖRE MÜŞTERİ LİSTELEME
        public List<MusteriDTO> Listele(
            MusteriAramaKriterleriDTO aramaKriterleri)
        {
            // Kriterler opsiyonel olduğu için
            // zorunlu alan kontrolü yapılmaz.
            return _musteriRepository.Listele(
                aramaKriterleri);
        }

        // 4. MÜŞTERİ GÜNCELLEME
        public void Guncelle(MusteriDTO dto)
        {
            RealityCheck(dto);

            MusteriDTO? mevcutMusteri =
                _musteriRepository.GetirById(dto.Id);

            if (mevcutMusteri is null)
            {
                throw new KeyNotFoundException(
                    $"{dto.Id} ID'li müşteri bulunamadı.");
            }

            // Güncellemeyi yapan personelin sicili
            // frontend yerine backend'den alınır.
            dto.RecordUser =
                _aktifPersonelServis.SicilNoGetir();

            _musteriRepository.Guncelle(dto);
        }

        // 5. MÜŞTERİ SİLME
        public void Sil(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Geçersiz müşteri ID'si.");
            }

            MusteriDTO? mevcutMusteri =
                _musteriRepository.GetirById(id);

            if (mevcutMusteri is null)
            {
                throw new KeyNotFoundException(
                    $"{id} ID'li müşteri bulunamadı.");
            }

            _musteriRepository.Sil(id);
        }

        // DASHBOARD ÖZETİ
        public MusteriDashboardDTO GetirDashboardOzet()
        {
            return _musteriRepository
                .GetirDashboardOzet();
        }

        // ORTAK DOĞRULAMA METODU
        private void RealityCheck(MusteriDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(
                    nameof(dto),
                    "Müşteri bilgileri gönderilmelidir.");
            }

            // ORTAK ALANLAR

            if (string.IsNullOrWhiteSpace(dto.Ad))
            {
                throw new ArgumentException(
                    "Ad girilmesi zorunludur.");
            }

            dto.Ad = dto.Ad.Trim();

            if (dto.Ad.Length > 50)
            {
                throw new ArgumentException(
                    "Ad en fazla 50 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Soyad))
            {
                throw new ArgumentException(
                    "Soyad girilmesi zorunludur.");
            }

            dto.Soyad = dto.Soyad.Trim();

            if (dto.Soyad.Length > 50)
            {
                throw new ArgumentException(
                    "Soyad en fazla 50 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Eposta))
            {
                throw new ArgumentException(
                    "E-posta girilmesi zorunludur.");
            }

            dto.Eposta = dto.Eposta.Trim();

            if (dto.Eposta.Length > 50)
            {
                throw new ArgumentException(
                    "E-posta en fazla 50 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
            {
                throw new ArgumentException(
                    "Telefon numarası girilmesi zorunludur.");
            }

            dto.TelefonNo =
                dto.TelefonNo.Trim();

            if (dto.TelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "Telefon numarası en fazla 13 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(
                    dto.SubeSubeKodu))
            {
                throw new ArgumentException(
                    "Şube seçilmesi zorunludur.");
            }

            dto.SubeSubeKodu =
                dto.SubeSubeKodu
                    .Trim()
                    .ToUpperInvariant();

            if (dto.SubeSubeKodu.Length > 20)
            {
                throw new ArgumentException(
                    "Şube kodu en fazla 20 karakter olabilir.");
            }

            // MÜŞTERİ TİPİ KONTROLÜ

            if (dto.MusteriTipi ==
                MusteriTipiDurumlari.None)
            {
                throw new ArgumentException(
                    "Müşteri tipi seçilmelidir.");
            }

            if (!Enum.IsDefined(
                    typeof(MusteriTipiDurumlari),
                    dto.MusteriTipi))
            {
                throw new ArgumentException(
                    "Geçersiz müşteri tipi.");
            }

            // DURUM KONTROLÜ

            if (dto.DurumKodu ==
                AktifPasifDurumlari.None)
            {
                throw new ArgumentException(
                    "Aktif veya pasif durumu seçilmelidir.");
            }

            if (!Enum.IsDefined(
                    typeof(AktifPasifDurumlari),
                    dto.DurumKodu))
            {
                throw new ArgumentException(
                    "Geçersiz müşteri durum kodu.");
            }

            // BİREYSEL MÜŞTERİ KONTROLLERİ

            if (dto.MusteriTipi ==
                MusteriTipiDurumlari.Bireysel)
            {
                if (dto.DogumTarihi is null)
                {
                    throw new ArgumentException(
                        "Bireysel müşteriler için doğum tarihi zorunludur.");
                }

                if (string.IsNullOrWhiteSpace(dto.TCKN))
                {
                    throw new ArgumentException(
                        "Bireysel müşteriler için TCKN zorunludur.");
                }

                dto.TCKN = dto.TCKN.Trim();

                if (dto.TCKN.Length != 11 ||
                    !dto.TCKN.All(char.IsDigit))
                {
                    throw new ArgumentException(
                        "TCKN 11 haneli ve yalnızca rakamlardan oluşmalıdır.");
                }

                if (dto.Cinsiyet is null ||
                    dto.Cinsiyet ==
                    CinsiyetDurumlari.None)
                {
                    throw new ArgumentException(
                        "Bireysel müşteriler için cinsiyet seçilmelidir.");
                }

                if (!Enum.IsDefined(
                        typeof(CinsiyetDurumlari),
                        dto.Cinsiyet.Value))
                {
                    throw new ArgumentException(
                        "Geçersiz cinsiyet değeri.");
                }

                // Bireysel müşteride
                // kurumsal alanlar kullanılmaz.
                dto.VKN = null;
                dto.Unvan = null;
            }

            // KURUMSAL MÜŞTERİ KONTROLLERİ

            if (dto.MusteriTipi ==
                MusteriTipiDurumlari.Kurumsal)
            {
                if (string.IsNullOrWhiteSpace(dto.VKN))
                {
                    throw new ArgumentException(
                        "Kurumsal müşteriler için VKN zorunludur.");
                }

                dto.VKN = dto.VKN.Trim();

                if (dto.VKN.Length != 10 ||
                    !dto.VKN.All(char.IsDigit))
                {
                    throw new ArgumentException(
                        "VKN 10 haneli ve yalnızca rakamlardan oluşmalıdır.");
                }

                if (string.IsNullOrWhiteSpace(dto.Unvan))
                {
                    throw new ArgumentException(
                        "Kurumsal müşteriler için unvan zorunludur.");
                }

                dto.Unvan = dto.Unvan.Trim();

                if (dto.Unvan.Length > 50)
                {
                    throw new ArgumentException(
                        "Unvan en fazla 50 karakter olabilir.");
                }

                // Kurumsal müşteride
                // bireysel alanlar kullanılmaz.
                dto.TCKN = null;
                dto.DogumTarihi = null;
                dto.Cinsiyet = null;
            }
        }

        // TAM KAYIT İÇİN
        // İLETİŞİM BİLGİLERİ KONTROLÜ
        private void IletisimBilgileriniKontrolEt(
            MusteriIletisimFormDTO iletisim)
        {
            iletisim.EvTelefonNo =
                MetniTemizle(
                    iletisim.EvTelefonNo);

            iletisim.IsTelefonNo =
                MetniTemizle(
                    iletisim.IsTelefonNo);

            iletisim.EvAdres =
                MetniTemizle(
                    iletisim.EvAdres);

            iletisim.IsAdres =
                MetniTemizle(
                    iletisim.IsAdres);

            if (iletisim.EvTelefonNo is not null &&
                iletisim.EvTelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "Ev telefonu en fazla 13 karakter olabilir.");
            }

            if (iletisim.IsTelefonNo is not null &&
                iletisim.IsTelefonNo.Length > 13)
            {
                throw new ArgumentException(
                    "İş telefonu en fazla 13 karakter olabilir.");
            }

            if (iletisim.EvAdres is not null &&
                iletisim.EvAdres.Length > 100)
            {
                throw new ArgumentException(
                    "Ev adresi en fazla 100 karakter olabilir.");
            }

            if (iletisim.IsAdres is not null &&
                iletisim.IsAdres.Length > 100)
            {
                throw new ArgumentException(
                    "İş adresi en fazla 100 karakter olabilir.");
            }
        }

        // BOŞ İLETİŞİM ALANLARINI NULL YAPAR
        private static string? MetniTemizle(
            string? metin)
        {
            if (string.IsNullOrWhiteSpace(metin))
            {
                return null;
            }

            return metin.Trim();
        }

        public void idCheck(long id)
        {
            if (id <= 0)
            {
                throw new Exception(
                    "Geçersiz ID");
            }
        }
    }
}