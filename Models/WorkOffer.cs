using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models;


namespace BetterWithDona.Models
{
    public class WorkOffer
    {
        public SystemProperties Sys { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public Asset Offer { get; set; }
        public string Message { get; set; }

    }
}
