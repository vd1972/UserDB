using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using userdb.Models;

namespace userdb.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.Current_User_Id = HttpContext.Session.GetInt32("UserId");
            ViewBag.Is_Admin = HttpContext.Session.GetInt32("IsAdmin");
            return View();
        }
    }
}
