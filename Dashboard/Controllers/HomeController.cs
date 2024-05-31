using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Dashboard.Controllers
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
            return View();
        }
        
        public IActionResult Posts()
        {
            return View();
        }
        
        public IActionResult Comments()
        {
            return View();
        }
        
        public IActionResult Users()
        {
            return View();
        }
    }
}
