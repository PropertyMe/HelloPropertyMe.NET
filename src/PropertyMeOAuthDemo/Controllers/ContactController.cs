using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PropertyMeOAuthDemo.Models;

namespace PropertyMeOAuthDemo.Controllers;

public class ContactController(IOptions<IntegrationSettings> settings) : Controller
{
    public IActionResult Index()
    {
        var accessToken = HttpContext.Session.GetObject<string>("access_token");
        if (accessToken == null) {
            ViewBag.ErrorMessage = "Access token is missing";
            return View("Index");
        }
        return View("Index", new ChangedContactsRequest 
        {
            Timestamp = 1, 
            AccessToken = accessToken
        });
    }

    /// <summary>
    ///     Sends a GET request to PropertyMe API End point with the authorization header.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Index(ChangedContactsRequest request)
    {
        var endPoint = $"{settings.Value.PropertyMeApiEndPoint}/api/v1/contacts?TimeStamp={request.Timestamp}";

        using (var client = new HttpClient()) {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + request.AccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try {
                var response = await client.GetStringAsync(endPoint);
                ViewBag.Response = response;
            }
            catch(Exception ex) {
                ViewBag.ResponseHeading = "Unexpected error has occurred";
                ViewBag.Response = ex.Message;
            }

            return View("Index", request);
        }
    }
}