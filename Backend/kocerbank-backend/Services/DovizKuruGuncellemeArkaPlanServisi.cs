using System.Text.Json;
using kocerbank_backend.Models.DTOs;

namespace kocerbank_backend.Services
{
    public class DovizKuruGuncellemeArkaPlanServisi : BackgroundService
    {
        private static readonly TimeSpan GunlukCalismaSaati = new TimeSpan(16, 0, 0);

        private static readonly JsonSerializerOptions YazmaAyarlari = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DovizKuruGuncellemeArkaPlanServisi> _logger;

        public DovizKuruGuncellemeArkaPlanServisi(
            IServiceScopeFactory scopeFactory,
            IWebHostEnvironment environment,
            ILogger<DovizKuruGuncellemeArkaPlanServisi> logger
        )
        {
            _scopeFactory = scopeFactory;
            _environment = environment;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await KurlariGuncelleAsync(stoppingToken);

                TimeSpan gecikme = SonrakiCalismayaKalanSure(DateTime.Now);

                try
                {
                    await Task.Delay(gecikme, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task KurlariGuncelleAsync(CancellationToken cancellationToken)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();

                TcmbKurServisi tcmbKurServisi =
                    scope.ServiceProvider.GetRequiredService<TcmbKurServisi>();

                DovizKuruDosyasiDTO kurDosyasi =
                    await tcmbKurServisi.GuncelKurlariGetirAsync(cancellationToken);

                string kurDosyasiYolu = Path.Combine(
                    _environment.ContentRootPath,
                    "MockData",
                    "kur.json"
                );

                string json = JsonSerializer.Serialize(kurDosyasi, YazmaAyarlari);

                await File.WriteAllTextAsync(kurDosyasiYolu, json, cancellationToken);

                _logger.LogInformation(
                    "Döviz kurları TCMB'den güncellendi. Kur tarihi: {KurTarihi:yyyy-MM-dd}",
                    kurDosyasi.KurTarihi
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "TCMB'den döviz kuru güncellenemedi. Mevcut kur.json korunuyor."
                );
            }
        }

        private static TimeSpan SonrakiCalismayaKalanSure(DateTime simdi)
        {
            DateTime bugunkuCalismaZamani = simdi.Date + GunlukCalismaSaati;

            DateTime sonrakiCalismaZamani = simdi <= bugunkuCalismaZamani
                ? bugunkuCalismaZamani
                : bugunkuCalismaZamani.AddDays(1);

            return sonrakiCalismaZamani - simdi;
        }
    }
}
