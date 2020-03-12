using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class ResultDTO
    {
        [Required]
        [StringLength(10)]
        public string title { get; set; } = "title";
        public string data { get; set; } = "some data..";
    }
}
