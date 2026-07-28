using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusteriController : ControllerBase
    {
        private readonly MusteriService _musteriService;

        public MusteriController(MusteriService musteriService)
        {
            _musteriService = musteriService;
        }

    [HttpPost("Ekle")]
        public IActionResult Ekle([FromBody] MusteriDTO dto)
        {
            try
            {
                MusteriDTO eklenenMusteri = _musteriService.Ekle(dto);

                return CreatedAtAction(nameof(GetirById), new { id = eklenenMusteri.Id }, eklenenMusteri);
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
                MusteriDTO? musteri =
                    _musteriService.GetirById(id);

                if (musteri is null)
                {
                    return NotFound(new
                    {
                        mesaj = "Müşteri bulunamadı."
                    });
                }

                return Ok(musteri);
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
        public IActionResult Listele(
            [FromBody] MusteriAramaKriterleriDTO aramaKriterleri)
        {
            List<MusteriDTO> musteriler =
                _musteriService.Listele(aramaKriterleri);

            return Ok(musteriler);
        }

        [HttpPut("Güncelle/{id:long}")]
        public IActionResult Guncelle(
            long id,
            [FromBody] MusteriDTO dto)
        {
            try
            {
                dto.Id = id;

                _musteriService.Guncelle(dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new{mesaj = ex.Message});
            }
        }

        [HttpDelete("Sil/{id:long}")]
        public IActionResult Sil(long id)
        {
            try
            {
                _musteriService.Sil(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new{mesaj = ex.Message});
            }
        }
    }
}
