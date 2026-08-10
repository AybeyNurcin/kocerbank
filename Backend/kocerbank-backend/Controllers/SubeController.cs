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

        // 1. ŞUBE EKLEME
        [HttpPost("Ekle")]
        public IActionResult Ekle(
            [FromBody] SubeDTO dto)
        {
            try
            {
                SubeDTO eklenenSube =
                    _subeService.Ekle(dto);

                return Ok(eklenenSube);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    "Şube ekleme işlemi sırasında hata oluştu: "
                    + ex.Message);
            }
        }

        // 2. ID'YE GÖRE ŞUBE GETİRME
        [HttpPost("{id:long}")]
        public IActionResult GetirById(long id)
        {
            try
            {
                SubeDTO sube =
                    _subeService.GetirById(id);

                return Ok(sube);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    "Şube getirme işlemi sırasında hata oluştu: "
                    + ex.Message);
            }
        }

        // 3. KRİTERE GÖRE ŞUBE LİSTELEME
        [HttpPost("Listele")]
        public IActionResult Listele(
            [FromBody]
            SubeAramaKriterleriDTO aramaKriterleri)
        {
            try
            {
                List<SubeDTO> subeler =
                    _subeService.Listele(
                        aramaKriterleri);

                return Ok(subeler);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    "Şube listeleme işlemi sırasında hata oluştu: "
                    + ex.Message);
            }
        }

        // 4. ŞUBE GÜNCELLEME
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
            catch (Exception ex)
            {
                return BadRequest(
                    "Şube güncelleme işlemi sırasında hata oluştu: "
                    + ex.Message);
            }
        }

        // 5. ŞUBE SİLME
        [HttpDelete("{id:long}")]
        public IActionResult Sil(long id)
        {
            try
            {
                _subeService.Sil(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(
                    "Şube silme işlemi sırasında hata oluştu: "
                    + ex.Message);
            }
        }

        // 6. ŞUBE DASHBOARD ÖZETİ
        [HttpPost("DashboardOzet")]
        public IActionResult GetirDashboardOzet(
            [FromBody] DashboardFiltreDTO? filtre)
        {
            try
            {
                SubeDashboardDTO ozet =
                    _subeService.GetirDashboardOzet(
                        filtre);

                return Ok(ozet);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    });
            }
        }
    }
}