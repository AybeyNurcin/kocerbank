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
        [HttpPost("Ekle")]
        public IActionResult Ekle([FromBody] PersonelDTO dto)
        {
            try
            {
                PersonelDTO eklenenPersonel =
                    _personelService.Ekle(dto);

                PersonelSonucDTO sonuc =
                    eklenenPersonel.SonucaDonustur();

                return CreatedAtAction(
                    nameof(GetirById),
                    new { id = sonuc.Id },
                    sonuc);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 2. ID'YE GÖRE PERSONEL GETİRME
        [HttpGet("GetirById/{id:long}")]
        public IActionResult GetirById(long id)
        {
            try
            {
                PersonelDTO personel =
                    _personelService.GetirById(id);

                return Ok(
                    personel.SonucaDonustur());
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 3. PERSONEL GİRİŞİ
        [HttpPost("Login")]
        public IActionResult Login(
            [FromBody] PersonelLoginDTO dto)
        {
            try
            {
                PersonelDTO personel =
                    _personelService.Login(dto);

                return Ok(
                    personel.SonucaDonustur());
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

        // 4. KRİTERE GÖRE PERSONEL LİSTELEME
        [HttpPost("Listele")]
        public IActionResult Listele(
            [FromBody]
            PersonelAramaKriterleriDTO? aramaKriterleri)
        {
            try
            {
                List<PersonelSonucDTO> personeller =
                    _personelService
                        .Listele(aramaKriterleri)
                        .Select(personel =>
                            personel.SonucaDonustur())
                        .ToList();

                return Ok(personeller);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 5. PERSONEL GÜNCELLEME
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
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 6. PERSONEL ŞİFRE DEĞİŞTİRME
        [HttpPut("SifreDegistir/{id:long}")]
        public IActionResult SifreDegistir(
            long id,
            [FromBody] PersonelSifreDegistirDTO dto)
        {
            try
            {
                _personelService.SifreDegistir(
                    id,
                    dto);

                return NoContent();
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

        // 7. PERSONEL SİLME
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
                return BadRequest(new
                {
                    mesaj = ex.Message
                });
            }
        }

        // 8. PERSONEL DASHBOARD ÖZETİ
        [HttpPost("DashboardOzet")]
        public IActionResult GetirDashboardOzet(
            [FromBody] DashboardFiltreDTO? filtre)
        {
            try
            {
                PersonelDashboardDTO ozet =
                    _personelService
                        .GetirDashboardOzet(filtre);

                return Ok(ozet);
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