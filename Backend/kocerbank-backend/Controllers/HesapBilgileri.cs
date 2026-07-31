using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HesapController : ControllerBase
    {
        private readonly HesapService _hesapService;

        public HesapController(HesapService hesapService)
        {
            _hesapService = hesapService;
        }

        [HttpPost("Ekle")]
        public IActionResult Ekle([FromBody] HesapDTO dto)
        {
            try
            {
                HesapDTO eklenenHesap = _hesapService.Ekle(dto);

                return CreatedAtAction(nameof(GetirById), new { id = eklenenHesap.Id }, eklenenHesap);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesaj = ex.Message });
            }
        }

        [HttpPost("GetirById/{id:long}")]
        public IActionResult GetirById(long id)
        {
            try
            {
                HesapDTO? hesap = _hesapService.GetirById(id);

                if (hesap is null)
                {
                    return NotFound(new
                    {
                        mesaj = "Hesap bulunamadı."
                    });
                }

                return Ok(hesap);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        [HttpPost("listele")]
        public IActionResult Listele([FromBody] HesapAramaKriterleriDTO aramaKriterleri)
        {
            List<HesapDTO> hesaplar = _hesapService.Listele(aramaKriterleri);

            return Ok(hesaplar);
        }

        [HttpPut("Guncelle/{id:long}")]
        public IActionResult Guncelle(long id, [FromBody] HesapDTO dto)
        {
            try
            {
                dto.Id = id;

                _hesapService.Guncelle(dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }
    }
}