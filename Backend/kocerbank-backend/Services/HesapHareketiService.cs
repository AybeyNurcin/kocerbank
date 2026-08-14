using kocerbank_backend.DataAccess;
using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class HesapHareketiService
    {
        private readonly HesapHareketiRepository _hesapHareketiRepository;
        private readonly ParaTransferService _paraTransferService;

        public HesapHareketiService(
            HesapHareketiRepository hesapHareketiRepository,
            ParaTransferService paraTransferService)
        {
            _hesapHareketiRepository = hesapHareketiRepository;
            _paraTransferService = paraTransferService;
        }


        public List<HesapHareketiDTO> Listele(long hesapBilgileriId)
        {
            if (hesapBilgileriId <= 0)
            {
                throw new ArgumentException("Geçersiz hesap ID'si.");
            }

            List<HesapHareketiDTO> hareketler =
                _hesapHareketiRepository.Listele(hesapBilgileriId);

            /*
             * GİDEN TRANSFER HAREKETLERİNDE MASRAF TUTARI
             *
             * Masraf, hesap hareketi kaydında tutulmadığı
             * için ilgili transfer kaydından işlem türüne
             * göre anlık hesaplanır.
             */
            foreach (HesapHareketiDTO hareket in hareketler)
            {
                if (
                    hareket.HesapHareketiTipi ==
                        HesapHareketTipleri.GidenTransfer &&
                    hareket.ParaTransferiId.HasValue
                )
                {
                    hareket.KomisyonTutari =
                        _paraTransferService
                            .KomisyonTutariniHesapla(
                                hareket.ParaTransferiId.Value
                            );
                }
            }

            return hareketler;
        }
    }
}
