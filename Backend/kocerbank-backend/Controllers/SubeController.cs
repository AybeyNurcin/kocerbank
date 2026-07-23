/*
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
*/

using kocerbank_backend.DataAccess;
using kocerbank_backend.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubeController : ControllerBase
    {
        private readonly SubeRepository _subeRepository;

        public SubeController(
            SubeRepository subeRepository)
        {
            _subeRepository = subeRepository;
        }

        // POST /api/Sube/Ekle
        [HttpPost("Ekle")]
        public IActionResult Ekle(
            [FromBody] SubeDTO dto)
        {
            SubeDTO eklenenSube =
                _subeRepository.Ekle(dto);

            return Ok(eklenenSube);
        }

        // POST /api/Sube/Getir/15
        [HttpPost("Getir/{id:long}")]
        public IActionResult GetirById(long id)
        {
            SubeDTO? sube =
                _subeRepository.GetirById(id);

            if (sube is null)
            {
                return NotFound(new
                {
                    mesaj = "Şube bulunamadı."
                });
            }

            return Ok(sube);
        }

        // POST /api/Sube/Listele
        [HttpPost("Listele")]
        public IActionResult Listele(
            [FromBody] SubeDTO aramaKriterleri)
        {
            List<SubeDTO> subeler =
                _subeRepository.GetirListele(
                    aramaKriterleri);

            return Ok(subeler);
        }

        // PUT /api/Sube/Guncelle/15
        [HttpPut("Guncelle/{id:long}")]
        public IActionResult Guncelle(
            long id,
            [FromBody] SubeDTO dto)
        {
            dto.Id = id;

            _subeRepository.Guncelle(dto);

            return Ok(new
            {
                mesaj = "Şube başarıyla güncellendi."
            });
        }

        // DELETE /api/Sube/Sil/15
        [HttpDelete("Sil/{id:long}")]
        public IActionResult Sil(long id)
        {
            _subeRepository.Sil(id);

            return Ok(new
            {
                mesaj = "Şube başarıyla silindi."
            });
        }
    }
}