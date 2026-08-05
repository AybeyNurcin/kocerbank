using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParaTransferController : ControllerBase
    {
        private readonly ParaTransferServis
            _paraTransferServis;


        public ParaTransferController(
            ParaTransferServis paraTransferServis
        )
        {
            _paraTransferServis =
                paraTransferServis;
        }


        [HttpPost("ParaTransferiYap")]
        public IActionResult ParaTransferiYap(
            [FromBody] ParaTransferDTO dto
        )
        {
            try
            {
                ParaTransferDTO sonuc =
                    _paraTransferServis
                        .ParaTransferiYap(dto);

                return Ok(sonuc);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
        }

        [HttpPost("TransferBilgileriniGetir")]
        public IActionResult TransferBilgileriniGetir(
            [FromBody] ParaTransferDTO dto
        )
        {
            try
            {
                ParaTransferDTO sonuc =
                    _paraTransferServis
                        .TransferBilgileriniGetir(dto);

                return Ok(sonuc);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (FileNotFoundException ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        mesaj = ex.Message
                    }
                );
            }
        }
    }
}