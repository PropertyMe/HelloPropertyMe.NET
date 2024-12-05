using System.ComponentModel.DataAnnotations;

namespace PropertyMeOAuthDemo.Models
{
    public record AccessTokenRequest(
        [Required(ErrorMessage = "ClientId is required.")]
        string ClientId,
        [Required(ErrorMessage = "Client Secret is required.")]
        string ClientSecret);
}
