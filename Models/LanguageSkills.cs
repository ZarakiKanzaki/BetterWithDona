using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace BetterWithDona.Models
{
    public class LanguageSkills
    {
        public SystemProperties Sys { get; set; }
        public string EntryType { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}

