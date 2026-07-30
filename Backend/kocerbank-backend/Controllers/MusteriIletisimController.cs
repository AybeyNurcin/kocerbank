using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusteriIletisimController : ControllerBase
    {
        private readonly MusteriIletisimService _musteriIletisimService;

        public MusteriIletisimController(MusteriIletisimService musteriIletisimService)
        {
            _musteriIletisimService = musteriIletisimService;
        }
    
    [HttpPost("Ekle")]
        public IActionResult Ekle([FromBody] MusteriIletisimDTO dto)
        {
            try
            {
                MusteriIletisimDTO eklenenIletisim = _musteriIletisimService.Ekle(dto);

                return CreatedAtAction(nameof(GetirById), new { id = eklenenIletisim.Id }, eklenenIletisim);
            }
            catch (Exception ex)
            {
                return BadRequest(new{mesaj = ex.Message});
            }
        }
    
    [HttpPost("GetirById/{id:long}")]
        public IActionResult GetirById(long id)
        {
            try
            {
                MusteriIletisimDTO? iletisim =
                    _musteriIletisimService.GetirById(id);
                if (iletisim is null)
                {
                    return NotFound(new
                    {
                        mesaj = "İletişim bilgisi bulunamadı."
                    });
                }
                return Ok(iletisim);
            }
            catch (Exception ex) 
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }
    
    [HttpPut("Guncelle/{id:long}")]
        public IActionResult Guncelle(
            long id,
            [FromBody] MusteriIletisimAramaKriterleriDTO dto)
        {
            try
            {
                dto.MusteriBilgileriId = id;

                _musteriIletisimService.Guncelle(dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new{mesaj = ex.Message});
            }
        }
    }
}