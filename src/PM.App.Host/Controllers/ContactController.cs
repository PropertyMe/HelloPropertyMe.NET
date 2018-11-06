using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PM.Api.Host.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace PM.Api.Host.Controllers
{
    public class ContactController: Controller
    {
        private readonly IOptions<IntegrationSettings> settings;
        public ContactController(IOptions<IntegrationSettings> settings)
        {
            this.settings = settings;
        }

        public IActionResult Index()
        {
            return View("Index", new ChangedContactsRequest { Timestamp = 1, AccessToken = HttpContext.Session.GetObject<string>("access_token") });
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
}
