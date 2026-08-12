using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DovizKuruController : ControllerBase
    {
        private readonly DovizKuruService _dovizKuruServis;

        public DovizKuruController(DovizKuruService dovizKuruServis)
        {
            _dovizKuruServis = dovizKuruServis;
        }

        [HttpPost("GuncelKurlar")]
        public IActionResult GuncelKurlar()
        {
            try
            {
                DovizKuruDosyasiDTO kurlar = _dovizKuruServis.GuncelKurlariGetir();
                return Ok(kurlar);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesaj = ex.Message });
            }
        }
    }
}
