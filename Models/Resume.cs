using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace BetterWithDona.Models
{
    public class Resume
    {
        public SystemProperties Sys { get; set; }
        public string Title { get; set; }
        public Document About { get; set; }
        public DateTime Birthdate { get; set; }
        public Asset Photo { get; set; }
        public object Address { get; set; }
        public Asset CV { get; set; }
        public List<Contacts> Contacts { get; set; }
        public List<WorkExperience> WorkExperience { get; set; }
        public List<Education> Education { get; set; }
        public List<LanguageSkills> Skills1 { get; set; }
        public List<ProjectCertification> ProjectAndCertification { get; set; }
        public List<UsefulLinks> UsefulLinks { get; set; }
    }
}

