using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContainerApp.WebApp.Models;
using Newtonsoft.Json;

namespace ContainerApp.WebApp.Controllers
{
    public class WeatherForecastController : Controller
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _config;
        private static readonly HttpClient client = new HttpClient();

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<WeatherForecastModel> lst = new List<WeatherForecastModel>();
            string _urlApi = string.Empty;

            using (var httpClient = new HttpClient())
            {
                _urlApi = _config["WeatherApi"].ToString() + "/api/WeatherForecast";
                _logger.LogInformation("URL API = " + _urlApi);

                using (var response = await httpClient.GetAsync( _urlApi))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    lst = JsonConvert.DeserializeObject<List<WeatherForecastModel>>(apiResponse);
                }
            }
            return View(lst);
        }

        public IActionResult ViewConfig()
        {
            ViewBag.ApiAddress = _config["ApiAddress"].ToString();
            ViewBag.WeatherApi = _config["WeatherApi"].ToString();
            return View();
        }
    }
}