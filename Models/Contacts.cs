using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace BetterWithDona.Models
{
    public class Contacts
    {
        public SystemProperties Sys { get; set; }
        public string Contact { get; set; }
        public string ContactType { get; set; }
    }
}

