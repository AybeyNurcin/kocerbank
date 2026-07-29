using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace kocerbank_backend.Services
{
    public class PersonelService
    {
        private readonly PersonelRepository _personelRepository;
        private readonly PasswordHasher<PersonelDTO> _passwordHasher;

        public PersonelService(PersonelRepository personelRepository)
        {
            _personelRepository = personelRepository;
            _passwordHasher = new PasswordHasher<PersonelDTO>();
        }

        // 1. PERSONEL EKLEME
        public PersonelDTO Ekle(PersonelDTO dto)
        {
            PersonelBilgileriniKontrolEt(dto);

                dto.Sifre = _passwordHasher.HashPassword(dto, dto.Sifre);

                return _personelRepository.Ekle(dto);
        }

        // 2. ID'YE GÖRE PERSONEL GETİRME
        public PersonelDTO? GetirById(long id)
        {
            var personel = _personelRepository.GetirById(id);
            if (personel == null)
            {
                throw new KeyNotFoundException($"Personel bulunamadı: ID = {id}");
            }
            IdKontrolEt(id);

            return _personelRepository.GetirById(id);
        }

        public List<PersonelDTO> Listele(PersonelAramaKriterleriDTO aramaKriterleri)
        {
            return _personelRepository.GetirListele(aramaKriterleri);
        }

        // 4. PERSONEL GÜNCELLEME
        public void Guncelle(PersonelDTO dto)
        {
            IdKontrolEt(dto.Id);

            PersonelBilgileriniKontrolEt(dto);

            PersonelDTO? mevcutPersonel = _personelRepository.GetirById(dto.Id);

            if (mevcutPersonel is null)
            {
                throw new KeyNotFoundException(
                    "Güncellenecek personel bulunamadı.");
            }

            _personelRepository.Guncelle(dto);
        }

        // 5. PERSONEL SİLME
        public void Sil(long id)
        {
            IdKontrolEt(id);

            PersonelDTO? mevcutPersonel =  _personelRepository.GetirById(id);

            if (mevcutPersonel is null)
            {
                throw new KeyNotFoundException(
                    "Silinecek personel bulunamadı.");
            }

            _personelRepository.Sil(id);
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

            if (string.IsNullOrWhiteSpace(dto.Soyad))
            {
                throw new ArgumentException(
                    "Personel soyadı boş bırakılamaz.");
            }

            // TCKN kontrolü (Genelde 11 hane olmalıdır)
            if (dto.TCKN.ToString().Length != 11)
            {
                throw new ArgumentException(
                    "TC Kimlik Numarası 11 haneli olmalıdır.");
            }

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
            {
                throw new ArgumentException(
                    "Personel telefon numarası boş bırakılamaz.");
            }

            if (dto.DurumKodu == AktifPasifDurumlari.None)
            {
                throw new ArgumentException(
                    "Personel durumu Aktif veya Pasif olmalıdır.");
            }

            if (!Enum.IsDefined(
                    typeof(AktifPasifDurumlari),
                    dto.DurumKodu))
            {
                throw new ArgumentException(
                    "Geçersiz personel durum kodu gönderildi.");
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
                    "Personel ID değeri sıfırdan büyük olmalıdır.");
            }
        }
    }
}


