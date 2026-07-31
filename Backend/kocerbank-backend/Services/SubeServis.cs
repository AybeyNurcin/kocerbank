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

        // Önce zorunlu alanlar ve uzunluklar kontrol edilir.
        SubeBilgileriniKontrolEt(dto);

        // Kullanıcının girdiği bilgilerle listeleme filtresi hazırlanır.
        SubeAramaKriterleriDTO aramaKriterleri = new SubeAramaKriterleriDTO
        {
            SubeAdi = dto.SubeAdi,
            SubeTelefonNo = dto.SubeTelefonNo,
            SubeAdres = dto.SubeAdres,
        };

        // Aynı Service içindeki Listele metodu çağrılır.
        List<SubeDTO> bulunanSubeler =
            Listele(aramaKriterleri);

        // Bu bilgilerle eşleşen kayıt varsa INSERT yapılmaz.
        if (bulunanSubeler.Count > 0)
        {
            throw new InvalidOperationException(
                "Girilen bilgilere sahip bir şube zaten bulunmaktadır.");
        }

        // Eşleşen kayıt yoksa INSERT işlemi yapılır.
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
        public List<SubeDTO> Listele(SubeAramaKriterleriDTO aramaKriterleri)
        {
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

        // ID geçerli mi?
        IdKontrolEt(dto.Id);

        // Şube adı, telefon, adres ve durum kontrolleri
        SubeBilgileriniKontrolEt(dto);

        // Güncellenecek şube gerçekten var mı?
        SubeDTO? mevcutSube =
            _subeRepository.GetirById(dto.Id);

        if (mevcutSube is null)
        {
            throw new KeyNotFoundException(
                "Güncellenecek şube bulunamadı.");
        }

        // Yeni bilgilerle aynı başka bir şube var mı?
        // Şube kodu ve durum kodu duplicate kontrolüne dahil edilmez.
        SubeAramaKriterleriDTO aramaKriterleri = new SubeAramaKriterleriDTO
        {
            SubeAdi = dto.SubeAdi,
            SubeTelefonNo = dto.SubeTelefonNo,
            SubeAdres = dto.SubeAdres
        };

        List<SubeDTO> bulunanSubeler =
            Listele(aramaKriterleri);

        // Kayıt kendi kendisini duplicate saymamalı.
        bool baskaSubeVarMi =
            bulunanSubeler.Any(
                sube => sube.Id != dto.Id);

        if (baskaSubeVarMi)
        {
            throw new InvalidOperationException(
                "Girilen bilgilere sahip başka bir şube zaten bulunmaktadır.");
        }

        // Sorun yoksa güncelleme prosedürü çalıştırılır.
        _subeRepository.Guncelle(dto);
    }

        // 5. ŞUBE SİLME
    public void Sil(long id)
    {
        // ID geçerli mi ve bu ID'ye ait şube var mı?
        _ = GetirById(id);

        // Kayıt varsa silme prosedürü çalıştırılır.
        _subeRepository.Sil(id);
    }


         public SubeDashboardDTO GetirDashboardOzet()
        {
            return _subeRepository.GetirDashboardOzet();
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

        // ID KULLANAN METOTLARIN ORTAK KONTROLÜ
        private static void IdKontrolEt(long id)
        {
            if (id <= 0) 
            {
                throw new ArgumentException(
                    "Bu ID değerinde Şube değeri olamaz.");    
            }    
        }
    }
}