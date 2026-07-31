/*using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParaTransferiController : ControllerBase
    {
        private readonly ParaTransferiService _paraTransferiService;

        public ParaTransferiController(ParaTransferiService paraTransferiService)
        {
            _paraTransferiService = paraTransferiService;
        }

        // 1. PARA TRANSFERİ EKLEME
        // POST /api/ParaTransferi/Ekle
        [HttpPost("Ekle")]
        public IActionResult Ekle([FromBody] ParaTransferiDTO dto)
        {
            try
            {
                ParaTransferiDTO eklenenTransfer =
                    _paraTransferiService.Ekle(dto);

                return CreatedAtAction(
                    nameof(GetirById),
                    new { id = eklenenTransfer.Id },
                    eklenenTransfer);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 2. ID'YE GÖRE GETİR
        [HttpGet("GetirById/{id:long}")]
        public IActionResult GetirById(long id)
        {
            try
            {
                ParaTransferiDTO? transfer =
                    _paraTransferiService.GetirById(id);

                if (transfer is null)
                {
                    return NotFound(new
                    {
                        mesaj = "Para transferi bulunamadı."
                    });
                }

                return Ok(transfer);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 3. GÜNCELLE
        [HttpPut("Guncelle/{id:long}")]
        public IActionResult Guncelle(
            long id,
            [FromBody] ParaTransferiDTO dto)
        {
            try
            {
                dto.Id = id;

                _paraTransferiService.Guncelle(dto);

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
*/