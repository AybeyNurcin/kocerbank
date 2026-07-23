using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using kocerbank_backend.Models.DTOs;
using kocerbank_backend.DataAccess;

namespace kocerbank_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonelController : ControllerBase
    {
        private readonly PersonelRepository _repository;

        // Dependency Injection: Program.cs'de kaydettiğimiz Repository buraya otomatik enjekte edilir
        public PersonelController(PersonelRepository repository)
        {
            _repository = repository;
        }

        // 1. PERSONEL EKLEME (CREATE)
        // Dışarıdan gelen veriler JSON formatında [FromBody] ile DTO'ya dönüştürülür
        [HttpPost("Ekle")]
        public IActionResult Ekle([FromBody] PersonelDTO dto)
        {
            try
            {
                var yeniPersonel = _repository.Ekle(dto);
                // Başarılı olursa 200 OK ve oluşturulan datayı geri dön
                return Ok(yeniPersonel); 
            }
            catch (Exception ex)
            {
                // Hata durumunda 400 Bad Request ve hatanın sebebini dön
                return BadRequest("Kayıt sırasında bir hata oluştu: " + ex.Message);
            }
        }

        // 2. ID'YE GÖRE GETİR (READ)
        // URL'den dinamik olarak ID alıyoruz (Örn: /api/Personel/Getir/5)
        [HttpGet("Getir/{id}")]
        public IActionResult Getir(long id)
        {
            try
            {
                var personel = _repository.GetirById(id);
                
                if (personel == null)
                    return NotFound("Bu ID'ye ait personel bulunamadı."); // 404 Not Found dön
                
                return Ok(personel);
            }
            catch (Exception ex)
            {
                return BadRequest("Sorgulama sırasında hata oluştu: " + ex.Message);
            }
        }

        // 3. KRİTERE GÖRE LİSTELE (READ)
        // Arama kriterleri çok fazla alan içerdiği için URL (GET) yerine Body (POST) kullanmak daha güvenlidir
        [HttpPost("Listele")]
        public IActionResult Listele([FromBody] PersonelDTO kriterler)
        {
            try
            {
                var liste = _repository.GetirListele(kriterler);
                
                // Hiç kayıt yoksa boş liste yerine NotFound dönmeyi tercih edebiliriz
                if (liste == null || liste.Count == 0)
                    return NotFound("Kriterlere uygun personel bulunamadı.");

                return Ok(liste);
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme sırasında hata oluştu: " + ex.Message);
            }
        }

        // 4. GÜNCELLEME (UPDATE)
        [HttpPut("Guncelle")]
        public IActionResult Guncelle([FromBody] PersonelDTO dto)
        {
            try
            {
                // Güncellenecek personel var mı diye kontrol etmek iyi bir pratiktir
                var mevcutPersonel = _repository.GetirById(dto.Id);
                if (mevcutPersonel == null)
                    return NotFound("Güncellenmek istenen personel bulunamadı.");

                _repository.Guncelle(dto);
                return Ok("Personel başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return BadRequest("Güncelleme sırasında hata oluştu: " + ex.Message);
            }
        }

        // 5. SİLME (DELETE)
        [HttpDelete("Sil/{id}")]
        public IActionResult Sil(long id)
        {
            try
            {
                var mevcutPersonel = _repository.GetirById(id);
                if (mevcutPersonel == null)
                    return NotFound("Silinmek istenen personel zaten yok.");

                _repository.Sil(id);
                return Ok("Personel başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return BadRequest("Silme işlemi başarısız: " + ex.Message);
            }
        }
    }
}