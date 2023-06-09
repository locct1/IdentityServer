﻿using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public ITokenService _tokenService;
        public HomeController(ILogger<HomeController> logger, ITokenService tokenService)
        {
            _logger = logger;
            _tokenService = tokenService;
        }
        [Authorize]
        public async Task<IActionResult> Weather()
        {
            var data = new List<WeatherModel>();
            var token = await _tokenService.GetToken("weatherapi.read");
            using (var client = new HttpClient())
            {
                client.SetBearerToken(token.AccessToken);
                var result = client.GetAsync("https://localhost:5445/weatherforecast").Result;
                if (result.IsSuccessStatusCode)
                {
                    var model = await result.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<List<WeatherModel>>(model);
                    return View(data);
                }
                else
                {
                    throw new Exception("Failed to get Data from API");
                }
            }
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}