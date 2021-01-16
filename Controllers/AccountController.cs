using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetterWithDona.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;

        private readonly IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public AccountController(ILogger<AccountController> logger)
        {
            this.logger = logger;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == config.GetValue<string>("fakeUser") && password == config.GetValue<string>("fakePwd"))
            {
                return RedirectToAction("Dashboard");

            }
            TempData["err"]= "Invalid Credentials";
            return View();
        }
    }
}
