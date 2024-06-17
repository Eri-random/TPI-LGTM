using System.ComponentModel.DataAnnotations;

namespace backend.api.Models.RequestModels
{
    public class ImageClassificationRequestModel
    {
        [Required]
        public IFormFile Image { get; set; }
    }
}
