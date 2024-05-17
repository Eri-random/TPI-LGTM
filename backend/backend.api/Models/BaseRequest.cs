using System.ComponentModel.DataAnnotations;

namespace backend.api.Models
{
    public class BaseRequest
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
