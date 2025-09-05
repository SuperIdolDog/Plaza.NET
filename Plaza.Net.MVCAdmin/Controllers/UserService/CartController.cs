using Microsoft.AspNetCore.Mvc;

namespace Plaza.Net.MVCAdmin.Controllers.Service
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
