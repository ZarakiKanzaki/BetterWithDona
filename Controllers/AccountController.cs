using BetterWithDona.Models;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BetterWithDona.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly IContentfulClient client;
        private readonly IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public AccountController(ILogger<AccountController> logger, IContentfulClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        // on each of all this actionresul you neec [Authorize()]
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult EditResume()
        {
            return View();
        }

        public async Task<IActionResult> HandleOffers()
        {
            IEnumerable<WorkOffer> workOffers = await client.GetEntriesByType<WorkOffer>("workOffer");
            return View(workOffers);
        }

        public IActionResult EditAccount()
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
            TempData["err"] = "Invalid Credentials";
            return View();
        }
        #region support class
        private class deserializedOffer
        {
            public string type { get; set; }
            public string linkType { get; set; }
            public string id { get; set; }
        }
        #endregion
    }
}
