using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Insurance.Shared.Payload.Requests
{
    public class SurchargeUploadRequest
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public IFormFile SurchargeFile { get; set; } = null!;
    }
}
