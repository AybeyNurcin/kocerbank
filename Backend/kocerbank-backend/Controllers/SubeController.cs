using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubeController : ControllerBase
    {
        private readonly SubeService _subeService;

        public SubeController(SubeService subeService)
        {
            _subeService = subeService;
        }

        [HttpPost("Ekle")]
        public IActionResult Ekle([FromBody] SubeDTO dto)
        {
            try
            {
                SubeDTO eklenenSube =
                    _subeService.Ekle(dto);

                return CreatedAtAction(
                    nameof(GetirById),
                    new { id = eklenenSube.Id },
                    eklenenSube);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        [HttpPost("{id:long}")]
        public IActionResult GetirById(long id)
        {
            try
            {
                SubeDTO? sube =
                    _subeService.GetirById(id);

                if (sube is null)
                {
                    return NotFound(new
                    {
                        mesaj = "Şube bulunamadı."
                    });
                }

                return Ok(sube);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        [HttpPost("listele")]
        public IActionResult Listele(
            [FromBody] SubeDTO aramaKriterleri)
        {
            List<SubeDTO> subeler =
                _subeService.Listele(aramaKriterleri);

            return Ok(subeler);
        }

        [HttpPut("{id:long}")]
        public IActionResult Guncelle(
            long id,
            [FromBody] SubeDTO dto)
        {
            try
            {
                dto.Id = id;

                _subeService.Guncelle(dto);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    mesaj = ex.Message
                });
            }
        }

        [HttpDelete("{id:long}")]
        public IActionResult Sil(long id)
        {
            try
            {
                _subeService.Sil(id);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    mesaj = ex.Message
                });
            }
        }
    }
}