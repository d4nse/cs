using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RecipeServer.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace RecipeServer.Controllers;

[EnableRateLimiting("PerClientRateLimit")]
public class HomeController : Controller {

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger) {
        _logger = logger;
    }

    public IActionResult Index() {
        return View(new RecipeRequest());
    }

    [HttpPost]
    public IActionResult GenerateRecipe(RecipeRequest request) {
        var client_ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation(
            "Generating recipe for {client_ip} with ingredients: {Ingredients}",
            client_ip, request.Ingredients);
        DumbRecipeGenerator generator = new(request);
        request = generator.Generate();
        return View("Index", request);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None,
                   NoStore = true)]
    public IActionResult Error() {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ??
                                             HttpContext.TraceIdentifier });
    }
}
