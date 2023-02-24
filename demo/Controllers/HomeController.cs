using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using demo.Models;
using Microsoft.FeatureManagement;

namespace demo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _config;
    private readonly IFeatureManager _feature;

    public HomeController(ILogger<HomeController> logger, 
    IConfiguration config,
    IFeatureManager feature)
    {
        _logger = logger;
        _config = config;
        _feature = feature;
    }

    public IActionResult Index()
    {
        var showsecret = _feature.IsEnabledAsync("showsecret").Result;
        var m = new InfoModel(){ 
            Title = _config["Demo:title"],
            Text = _config["Demo:text"],
            Secret = showsecret ? _config["secret"]: "no secret"
            };
        return View(m);
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
