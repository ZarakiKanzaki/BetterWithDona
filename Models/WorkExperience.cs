using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace BetterWithDona.Models
{
    public class WorkExperience
    {
        public SystemProperties Sys { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public Document JobDescription { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public Location Location { get; set; }
    }
}

