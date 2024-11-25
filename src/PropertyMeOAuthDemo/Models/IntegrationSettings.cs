namespace PropertyMeOAuthDemo.Models;

public class IntegrationSettings
{
    /// <summary>
    ///     The endpoint that hosts the PropertyMe API.
    /// </summary>
    public string? PropertyMeApiEndPoint { get; set; }

    /// <summary>
    ///     Base-address of your identity server
    /// </summary>
    public string? AuthorityUrl { get;set; }

    /// <summary>
    ///     The location to redirect after successfully login. This must be same as registered in Identity server.
    /// </summary>
    public string? RedirectUrl { get;set; }
}