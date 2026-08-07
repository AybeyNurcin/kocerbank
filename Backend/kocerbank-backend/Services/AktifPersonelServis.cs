namespace kocerbank_backend.Services
{
    public class AktifPersonelServis
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AktifPersonelServis(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string SicilNoGetir()
        {
            var httpContext =
                _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException(
                    "HTTP isteği bulunamadı.");
            }

            var sicilNo =
                httpContext.Request.Headers[
                    "X-Personel-Sicil"
                ].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(sicilNo))
            {
                throw new UnauthorizedAccessException(
                    "Oturum açmış personel bilgisi bulunamadı.");
            }

            return sicilNo.Trim();
        }
    }
}