namespace kocerbank_backend.Services
{
    // HESAP/MUSTERI/PERSONEL/SUBE SERVİSLERİNDE TEKRAR EDEN
    // ID DOĞRULAMA, "KAYIT BULUNAMADI" VE RECORDUSER DAMGALAMA
    // MANTIĞI İÇİN ORTAK TABAN SINIF.
    public abstract class BaseCrudService
    {
        private readonly AktifPersonelService _aktifPersonelServis;

        protected BaseCrudService(
            AktifPersonelService aktifPersonelServis)
        {
            _aktifPersonelServis = aktifPersonelServis;
        }

        // ID DEĞERİNİN GEÇERLİ OLUP OLMADIĞINI KONTROL EDER
        protected static void IdKontrolEt(
            long id,
            string hataMesaji)
        {
            if (id <= 0)
            {
                throw new ArgumentException(hataMesaji);
            }
        }

        // KAYDIN BULUNUP BULUNMADIĞINI KONTROL EDER,
        // BULUNAMAZSA KeyNotFoundException FIRLATIR.
        protected static T KaydiBulunduMuKontrolEt<T>(
            T? kayit,
            string hataMesaji)
            where T : class
        {
            if (kayit is null)
            {
                throw new KeyNotFoundException(hataMesaji);
            }

            return kayit;
        }

        // GİRİŞ YAPAN PERSONELİN SİCİLİNİ DÖNDÜRÜR.
        // RECORDUSER ALANININ DAMGALANMASI İÇİN KULLANILIR.
        protected string GirisYapanPersonelSicili()
        {
            return _aktifPersonelServis.SicilNoGetir();
        }
    }
}
