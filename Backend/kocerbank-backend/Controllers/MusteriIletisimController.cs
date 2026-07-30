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
    }
}