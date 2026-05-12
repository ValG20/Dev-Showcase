using System.Diagnostics;
using EditorialUCR.Models;
using Microsoft.AspNetCore.Mvc;


 

namespace EditorialUCR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() 
        {
            return View("~/Views/Editoriales/Fichas/BusquedaDeFicha.cshtml");
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

        ////public class SolicitudPublicacionController : Controller
        ////{
        ////    public IActionResult Index() => View(); // ahora sí la encuentra por convención
        ////}
        
    }
}
