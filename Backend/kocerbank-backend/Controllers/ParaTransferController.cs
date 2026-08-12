using kocerbank_backend.Enums;
using kocerbank_backend.Models.DTOs;
using kocerbank_backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace kocerbank_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParaTransferController : ControllerBase
    {
        private readonly ParaTransferService
            _paraTransferServis;


        public ParaTransferController(
            ParaTransferService paraTransferServis
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

        [HttpPost("TekHesapBilgisiGetir")]
        public IActionResult TekHesapBilgisiGetir(
            [FromQuery] string iban,
            [FromQuery] TransferKanallari kanal
        )
        {
            try
            {
                HesapDTO sonuc =
                    _paraTransferServis
                        .TekHesapBilgisiGetir(iban, kanal);

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

        [HttpPost("TransferDetayiGetir/{id:long}")]
        public IActionResult TransferDetayiGetir(
            long id
        )
        {
            try
            {
                ParaTransferiDetayDTO sonuc =
                    _paraTransferServis
                        .TransferDetayiGetir(id);

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