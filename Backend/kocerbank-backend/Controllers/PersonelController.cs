/*
using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonelController : ControllerBase
    {
        private readonly PersonelService _personelService;

        public PersonelController(PersonelService personelService)
        {
            _personelService = personelService;
        }

        // 1. PERSONEL EKLEME
        // POST /api/Personel
        [HttpPost]
        public IActionResult Ekle([FromBody] PersonelDTO dto)
        {
            try
            {
                PersonelDTO eklenenPersonel =
                    _personelService.Ekle(dto);

                return CreatedAtAction(
                    nameof(GetirById),
                    new { id = eklenenPersonel.Id },
                    eklenenPersonel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 2. ID'YE GÖRE PERSONEL GETİRME
        // GET /api/Personel/5
        [HttpGet("{id:long}")]
        public IActionResult GetirById(long id)
        {
            try
            {
                PersonelDTO? personel =
                    _personelService.GetirById(id);

                if (personel is null)
                {
                    return NotFound(new
                    {
                        mesaj = "Personel bulunamadı."
                    });
                }

                return Ok(personel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 3. KRİTERE GÖRE PERSONEL LİSTELEME
        // POST /api/Personel/listele
        [HttpPost("listele")]
        public IActionResult Listele(
            [FromBody] PersonelDTO aramaKriterleri)
        {
            List<PersonelDTO> personeller =
                _personelService.Listele(aramaKriterleri);

            return Ok(personeller);
        }

        // 4. PERSONEL GÜNCELLEME
        // PUT /api/Personel/5
        [HttpPut("{id:long}")]
        public IActionResult Guncelle(
            long id,
            [FromBody] PersonelDTO dto)
        {
            try
            {
                dto.Id = id;

                _personelService.Guncelle(dto);

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

        // 5. PERSONEL SİLME
        // DELETE /api/Personel/5
        [HttpDelete("{id:long}")]
        public IActionResult Sil(long id)
        {
            try
            {
                _personelService.Sil(id);

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