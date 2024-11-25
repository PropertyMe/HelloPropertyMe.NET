using System.ComponentModel.DataAnnotations;

namespace PropertyMeOAuthDemo.Models;

public class BaseRequest
{
    [Required(ErrorMessage = "Access Token is required.")]
    public string? AccessToken { get; set; }
}