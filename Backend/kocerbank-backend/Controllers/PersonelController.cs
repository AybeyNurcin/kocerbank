
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
        [HttpPost("Ekle")]
            public IActionResult Ekle([FromBody] PersonelDTO dto)
            {
                try
                {
                    PersonelDTO eklenenPersonel = _personelService.Ekle(dto);

                    return CreatedAtAction(nameof(GetirById), new { id = eklenenPersonel.Id }, eklenenPersonel);
                }
                catch (Exception ex)
                {
                    return BadRequest(new{mesaj = ex.Message});
                }
            }

        [HttpGet("GetirById/{id:long}")]
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
            catch (Exception ex) 
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] PersonelLoginDTO dto)
        {
            try
            {
                PersonelDTO personel = _personelService.Login(dto);

                return Ok(personel);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
                {
                    mesaj = ex.Message
                });
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
            [FromBody] PersonelAramaKriterleriDTO aramaKriterleri)
        {
            List<PersonelDTO> personeller =
                _personelService.Listele(aramaKriterleri);

            return Ok(personeller);
        }

        [HttpPut("Güncelle/{id:long}")]
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
            catch (Exception ex)
            {
                return BadRequest(new{mesaj = ex.Message});
            }
        }


        [HttpPut("SifreDegistir/{id:long}")]
        public IActionResult SifreDegistir(
            long id,
            [FromBody] PersonelSifreDegistirDTO dto)
        {
            try
            {
                _personelService.SifreDegistir(id, dto);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mesaj = ex.Message });
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
                _personelService.Sil(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new{mesaj = ex.Message});
            }
        }

        [HttpGet("DashboardOzet")]
        public IActionResult GetirDashboardOzet()
        {
            try
            {
                PersonelDashboardDTO ozet = _personelService.GetirDashboardOzet();
                return Ok(ozet);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mesaj = ex.Message });
            }
        }
    }
}
