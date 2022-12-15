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
    public class TodoItemController : Controller
    {
        private readonly ILogger<TodoItemController> _logger;
        private readonly IConfiguration _config;
        private static readonly HttpClient client = new HttpClient();

        public TodoItemController(ILogger<TodoItemController> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Controller:TodoItemController - Method:Index");

                List<TodoItemModel> lst = new List<TodoItemModel>();
                string _urlApi = string.Empty;

                using (var httpClient = new HttpClient())
                {
                    _urlApi = _config["ApiAddress"].ToString() + "/api/TodoItems";
                    _logger.LogInformation("URL API = " + _urlApi);

                    using (var response = await httpClient.GetAsync( _urlApi))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        lst = JsonConvert.DeserializeObject<List<TodoItemModel>>(apiResponse);
                    }
                }

                _logger.LogInformation("Qtde: " + lst.Count().ToString());

                return View(lst);
            }
            catch (Exception ex)
            {
                _logger.LogError("ERROR: " + ex.Message);
                return Redirect("/Home/Error");
            }
        }

        public ActionResult Create()
        {
            _logger.LogInformation("Controller:TodoItemController - Method:Create");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TodoItemModel model)
        {
            try
            {
                _logger.LogInformation("Controller:TodoItemController - Method:Create");

                if (ModelState.IsValid)
                {
                    string _urlApi = _config["ApiAddress"].ToString() + "/api/TodoItems";
                    _logger.LogInformation("URL API = " + _urlApi);
                    _logger.LogInformation("MODEL = " + JsonConvert.SerializeObject(model));

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _urlApi);

                    string json = JsonConvert.SerializeObject(model);

                    request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    HttpClient http = new HttpClient();
                    HttpResponseMessage response = await http.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));     
                    }
                    else
                    {
                        _logger.LogError("ERROR: " + response); 
                        return Redirect("/Home/Error");
                    }
                }
                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.Message);
                return View();
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            _logger.LogInformation("Controller:TodoItemController - Method:Details");
            _logger.LogInformation("Parameter ID: " + id.ToString());
            
            string _urlApi = string.Empty;

            using (var httpClient = new HttpClient())
            {
                _urlApi = _config["ApiAddress"].ToString() + "/api/TodoItems/" + id;
                _logger.LogInformation("URL API = " + _urlApi);

                using (var response = await httpClient.GetAsync( _urlApi))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var todoItem = JsonConvert.DeserializeObject<TodoItemModel>(apiResponse);
                    return View(todoItem);
                }
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Controller:TodoItemController - Method:Delete");
            _logger.LogInformation("Parameter ID: " + id.ToString());
            
            string _urlApi = string.Empty;

            using (var httpClient = new HttpClient())
            {
                _urlApi = _config["ApiAddress"].ToString() + "/api/TodoItems/" + id;
                _logger.LogInformation("URL API = " + _urlApi);

                using (var response = await httpClient.GetAsync( _urlApi))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var todoItem = JsonConvert.DeserializeObject<TodoItemModel>(apiResponse);
                    return View(todoItem);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, TodoItemModel model)
        {
            try
            {
                _logger.LogInformation("Controller:TodoItemController - Method:Delete");
                _logger.LogInformation("Parameter ID: " + id.ToString());

                if (id > 0 )
                {
                    string _urlApi = _config["ApiAddress"].ToString() + "/api/TodoItems/" + id;
                    _logger.LogInformation("URL API = " + _urlApi);
                    _logger.LogInformation("MODEL = " + JsonConvert.SerializeObject(model));

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, _urlApi);

                    HttpClient http = new HttpClient();
                    HttpResponseMessage response = await http.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));     
                    }
                    else
                    {
                        _logger.LogError("ERROR: " + response);
                        return Redirect("/Home/Error");
                    }
                }
                else
                {
                    throw new Exception("Model is invalid");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.Message);
                return Redirect("/Home/Error");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            string _urlApi = string.Empty;

            _logger.LogInformation("Controller:TodoItemController - Method:Edit");
            _logger.LogInformation("Parameter ID: " + id.ToString());

            using (var httpClient = new HttpClient())
            {
                _urlApi = _config["ApiAddress"].ToString() + "/api/TodoItems/" + id;
                _logger.LogInformation("URL API = " + _urlApi);

                using (var response = await httpClient.GetAsync( _urlApi))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var todoItem = JsonConvert.DeserializeObject<TodoItemModel>(apiResponse);
                    return View(todoItem);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, TodoItemModel model)
        {
            try
            {
                _logger.LogInformation("Controller:TodoItemController - Method:Edit");
                _logger.LogInformation("Parameter ID: " + id.ToString());
                
                if (ModelState.IsValid)
                {
                    string _urlApi = _config["ApiAddress"].ToString() + "/api/TodoItems/" + id;
                    _logger.LogInformation("URL API = " + _urlApi);
                    _logger.LogInformation("MODEL = " + JsonConvert.SerializeObject(model));

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, _urlApi);

                    string json = JsonConvert.SerializeObject(model);

                    request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    HttpClient http = new HttpClient();
                    HttpResponseMessage response = await http.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));     
                    }
                    else
                    {
                        _logger.LogError("ERROR: " + response);
                        return Redirect("/Home/Error");
                    }
                }
                return View(model);
            }
            catch(Exception ex)
            {
                _logger.LogError("ERROR: " + ex.Message);
                return View();
            }
        }
    }
}