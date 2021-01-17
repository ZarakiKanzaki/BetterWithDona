using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetterWithDona.Models
{
    public class ServerMailCredentials
    {
        public string smtp { get; set; }
        public string userSmtp { get; set; }
        public string pwdSmtp { get; set; }
        public int port { get; set; }
    }
}
