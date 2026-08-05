using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class HesapHareketiService
    {
        private readonly HesapHareketiRepository _hesapHareketiRepository;

        public HesapHareketiService(HesapHareketiRepository hesapHareketiRepository)
        {
            _hesapHareketiRepository = hesapHareketiRepository;
        }


        public List<HesapHareketiDTO> Listele(long hesapBilgileriId)
        {
            if (hesapBilgileriId <= 0)
            {
                throw new ArgumentException("Geçersiz hesap ID'si.");
            }
            return _hesapHareketiRepository.Listele(hesapBilgileriId);
        }
    }
}
