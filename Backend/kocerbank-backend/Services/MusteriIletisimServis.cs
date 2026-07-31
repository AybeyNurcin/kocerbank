using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;


namespace kocerbank_backend.Services
{
    public class MusteriIletisimService
    {
        private readonly MusteriIletisimRepository _musteriIletisimRepository;

        public MusteriIletisimService(MusteriIletisimRepository musteriIletisimRepository)
        {
            _musteriIletisimRepository = musteriIletisimRepository;
        }

        public MusteriIletisimDTO Ekle(MusteriIletisimDTO dto)
        {
            return _musteriIletisimRepository.Ekle(dto);
        }
    

        public MusteriIletisimDTO? GetirById(long id)
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

        public void Guncelle(MusteriIletisimAramaKriterleriDTO dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto), "İletişim bilgileri gönderilmelidir.");
            }

            if (dto.MusteriBilgileriId <= 0)
            {
                throw new ArgumentException("Geçersiz müşteri ID'si.");
            }

            if (string.IsNullOrWhiteSpace(dto.TelefonNo))
            {
                throw new ArgumentException("Cep telefonu girilmesi zorunludur.");
            }

            dto.TelefonNo = dto.TelefonNo.Trim();

            if (dto.TelefonNo.Length > 13)
            {
                throw new ArgumentException("Cep telefonu en fazla 13 karakter olabilir.");
            }

            if (string.IsNullOrWhiteSpace(dto.Eposta))
            {
                throw new ArgumentException("E-posta girilmesi zorunludur.");
            }

            dto.Eposta = dto.Eposta.Trim();

            if (dto.Eposta.Length > 50)
            {
                throw new ArgumentException("E-posta en fazla 50 karakter olabilir.");
            }

            dto.EvTelefonNo = string.IsNullOrWhiteSpace(dto.EvTelefonNo) ? null : dto.EvTelefonNo.Trim();
            dto.IsTelefonNo = string.IsNullOrWhiteSpace(dto.IsTelefonNo) ? null : dto.IsTelefonNo.Trim();
            dto.EvAdres = string.IsNullOrWhiteSpace(dto.EvAdres) ? null : dto.EvAdres.Trim();
            dto.IsAdres = string.IsNullOrWhiteSpace(dto.IsAdres) ? null : dto.IsAdres.Trim();

            if (dto.EvTelefonNo is not null && dto.EvTelefonNo.Length > 13)
            {
                throw new ArgumentException("Ev telefonu en fazla 13 karakter olabilir.");
            }

            if (dto.IsTelefonNo is not null && dto.IsTelefonNo.Length > 13)
            {
                throw new ArgumentException("İş telefonu en fazla 13 karakter olabilir.");
            }

            if (dto.EvAdres is not null && dto.EvAdres.Length > 100)
            {
                throw new ArgumentException("Ev adresi en fazla 100 karakter olabilir.");
            }

            if (dto.IsAdres is not null && dto.IsAdres.Length > 100)
            {
                throw new ArgumentException("İş adresi en fazla 100 karakter olabilir.");
            }

            MusteriIletisimDTO? mevcutIletisim = _musteriIletisimRepository.GetirById(dto.MusteriBilgileriId);

            if (mevcutIletisim is null)
            {
                throw new KeyNotFoundException($"{dto.MusteriBilgileriId} ID'li iletişim bilgisi bulunamadı.");
            }

            _musteriIletisimRepository.Guncelle(dto);
        }

    }
}
