namespace kocerbank_backend.Models.DTOs
{
    public class BaseDTO
    {
        public long Id { get; set; }
        public string RecordUser { get; set; }  
        public DateTime RecordDate { get; set; }
    }
}

