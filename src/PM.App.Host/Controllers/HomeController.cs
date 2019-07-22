using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PM.Api.Host.Models;
using System.Threading.Tasks;

namespace PM.Api.Host.Controllers
{
    public class HomeController: Controller
    {
        private const string accessTokenKey = "accesstokenrequest";
        private readonly IOptions<IntegrationSettings> settings;
        private readonly HttpContext httpContext;

        public HomeController(IOptions<IntegrationSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            this.settings = settings;
            httpContext = httpContextAccessor.HttpContext;
        }

        public IActionResult Index()
        {
            var request = HttpContext.Session.GetObject<AccessTokenRequest>(accessTokenKey);
            if(request == null) {
                request = new AccessTokenRequest();
            }

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
            httpContext.Session.SetObject(accessTokenKey, request);

            /// Redirects to the IDP server for authentication. 
            // var endPoint = $"{settings.Value.AuthorityUrl}/connect/authorize?response_type=code&state=&client_id={request.ClientId}&scope=contact:read%20activity:read%20property:read%20communication:read%20transaction:read&redirect_uri={settings.Value.RedirectUrl}";
            var endPoint = $"{settings.Value.AuthorityUrl}/connect/authorize?response_type=code&state=&client_id={request.ClientId}&scope=transaction:read%20activity:read%20contact:read%20communication:write%20activity:write%20communication:read%20transaction:write%20contact:write%20property:write%20property:read&redirect_uri={settings.Value.RedirectUrl}";
            // var endPoint = $"{settings.Value.AuthorityUrl}/connect/authorize?response_type=code&state=&client_id={request.ClientId}&scope=contact:read&redirect_uri={settings.Value.RedirectUrl}";
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
            var accessTokenRequest = HttpContext.Session.GetObject<AccessTokenRequest>(accessTokenKey);

            var disco = await DiscoveryClient.GetAsync($"{settings.Value.AuthorityUrl}");
            if (disco.IsError) {
                ViewData["Message"] = "Unable to connect to IDP server";
                return View();
            }

            var tokenClient = new TokenClient(disco.TokenEndpoint, accessTokenRequest.ClientId, accessTokenRequest.ClientSecret);
            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(code, $"{settings.Value.RedirectUrl}");

            if (tokenResponse.IsError) {
                ViewData["Message"] = "Unable  get access token from the IDP server";
                return View();
            }

            ///For demo purposes we are storing the the access token in a session.
            httpContext.Session.SetObject("access_token", tokenResponse.Json["access_token"]);

            ViewData["Message"] = "Below is the access token that can be used to request data from PropertyMe API's";

            return View("Callback", tokenResponse.Json.ToString());
        }
    }
}
