using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace BetterWithDona.Models
{
    public class ProjectCertification
    {
        public SystemProperties Sys { get; set; }
        public string EntryType { get; set; }
        public string Title { get; set; }
        public object Description { get; set; }
        public string Url { get; set; }
    }
}

