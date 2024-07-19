using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace CallByIPAddress.Controllers
{
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index()
        {
            try
            {
                string userIpAddress = GetClientIpAddress(HttpContext);
                ViewBag.userIpAddress = userIpAddress;


                string userCountry = await GetUserCountryAsync(userIpAddress);


                ViewBag.UserCountry = userCountry;

                return View();
            }catch(Exception e)
            {
                ViewBag.UserCountry = "FromErr";
                ViewBag.err = e.Message;
                return View();
            }
        }

        private string GetClientIpAddress(HttpContext context)
        {
           string ipAddress = Response.HttpContext.Connection?.RemoteIpAddress.ToString();
            if (ipAddress =="::1")
            {
                ipAddress = Dns.GetHostEntry(Dns.GetHostName())?.AddressList[1].ToString();
            }
            return ipAddress ; // Default to localhost if no IP found
        }

        public async Task<string> GetUserCountryAsync(string ipAddress)
        {
            //if (ipAddress == "::1")
            //    ipAddress = "82.212.78.93";
            string url = $"https://ipinfo.io/{ipAddress}?token=ef3fd1b4c69c00";
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(url);
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                ViewBag.data = data +" "+ipAddress;
                string country = data.country;
                return country.Trim();
            }
        }
    }
}
