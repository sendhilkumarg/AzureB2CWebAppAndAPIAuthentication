using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace B2CWebApplication.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var apiurl = @"http://localhost:14111/api/values";

            string returnFromFirstLayer = string.Empty;
            string returnedFromSecondLayer = string.Empty;
            string token = string.Empty;
            try
            {
                returnFromFirstLayer = $"This is returned by CrmCallerFirstLayerController @ {DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss")}";
                string webApiUrl = @"http://localhost:14111/api/values";

                

                token = Request.Headers["Authorization"];
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get,
                    webApiUrl);
               // request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("Bearer", ""));
/*                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    returnedFromSecondLayer = "[SUCCESS]  " + responseContent;
                }
                else
                {
                    returnedFromSecondLayer = "[FAILED]  " + response.StatusCode;
                }*/
               // string at = await GetAccessToken();


            }
            catch (Exception ex)
            {
                returnedFromSecondLayer = ex.Message + "|" + ex.StackTrace;
            }


            return View();
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = String.Format("Claims available for the user {0}", (User.FindFirst("name")?.Value));
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public async Task<IActionResult> ReadFromAPI()
        {

            var result = string.Empty;
            try
            {
                var webApiUrl = @"http://localhost:14111/api/values";
                var token = await HttpContext.Authentication.GetTokenAsync("id_token");

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get,
                    webApiUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.Replace("Bearer", ""));
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    result = "[SUCCESS]  " + responseContent;
                }
                else
                {
                    result = "[FAILED]  " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message + "|" + ex.StackTrace;
            }


            return View("ReadFromAPI",result);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
