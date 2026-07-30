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
    }
}
