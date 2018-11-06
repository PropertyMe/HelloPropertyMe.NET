using System.ComponentModel.DataAnnotations;

namespace PM.Api.Host.Models
{
    public class AccessTokenRequest
    {
        [Required(ErrorMessage = "ClientId is required.")]
        public string ClientId { get; set; }
        [Required(ErrorMessage = "Client Secret is required.")]
        public string ClientSecret { get; set; }
    }
}
