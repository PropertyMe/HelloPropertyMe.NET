using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PropertyMeOAuthDemo.Models;

namespace PropertyMeOAuthDemo.Controllers;

public class HomeController: Controller
{
    private const string AccessTokenKey = "accesstokenrequest";
    private readonly IOptions<IntegrationSettings> _settings;
    private readonly HttpContext _httpContext;

    public HomeController(IOptions<IntegrationSettings> settings, IHttpContextAccessor httpContextAccessor)
    {
        _settings = settings;
        _httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("httpContextAccessor.HttpContext is null");
    }

    public IActionResult Index()
    {
        var request = HttpContext.Session.GetObject<AccessTokenRequest>(AccessTokenKey) ?? new AccessTokenRequest(String.Empty, String.Empty);

        return View(request);
    }

    /// <summary>
    ///     Redirects the client to the Identity server for authetication
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Index(AccessTokenRequest request)
    {
        if (!ModelState.IsValid) {
            return View(request);
        }

        ///For demo purposes this is stored in a session.
        _httpContext.Session.SetObject(AccessTokenKey, request);

       /// Redirects to the IDP server for authentication. 
        var endPoint = $"{_settings.Value.AuthorityUrl}/connect/authorize?response_type=code&state=&client_id={request.ClientId}&scope=contact%3Aread%20property%3Aread%20communication%3Aread%20transaction%3Aread%20offline_access&redirect_uri={_settings.Value.RedirectUrl}";
        return Redirect(endPoint);
    }

    /// <summary>
    ///     Redirected from the identity server after successfull authentication. The code recieved is used to get an access token.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<IActionResult> CallBack(string code)
    {
        ///For demo purposes this is being retrived from a session.
        var accessTokenRequest = HttpContext.Session.GetObject<AccessTokenRequest>(AccessTokenKey);
        if (accessTokenRequest == null)
        {
            ViewData["Message"] = "Access token missing from session";
            return View();
        }
        
        var disco = await DiscoveryClient.GetAsync($"{_settings.Value.AuthorityUrl}");
        if (disco.IsError) {
            ViewData["Message"] = "Unable to connect to IDP server";
            return View();
        }

        var tokenClient = new TokenClient(disco.TokenEndpoint, accessTokenRequest.ClientId,
            accessTokenRequest.ClientSecret);
        var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(code, $"{_settings.Value.RedirectUrl}");

        if (tokenResponse.IsError)
        {
            ViewData["Message"] = "Unable  get access token from the IDP server";
            return View();
        }

        ///For demo purposes we are storing the the access token in a session.
        _httpContext.Session.SetObject("access_token", tokenResponse.Json["access_token"].ToString());

        ViewData["Message"] = "Below is the access token that can be used to request data from PropertyMe API's";

        return View("Callback", tokenResponse.Json.ToString());
    }
}