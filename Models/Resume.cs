using System.ComponentModel.DataAnnotations;

namespace MyHRExample.Models
{
    public class Resume
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Ім'я кандидата")]
        public string CandidateName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Контактні дані")]
        public string ContactInfo { get; set; } = string.Empty;

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string S3Key { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}