using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSMWebServices.Helpers
{
    public class AppSettings
    {
        public string SqlServerHostName { get; set; }
        public string SqlServerPost { get; set; }
        public string SqlServerCatalog { get; set; }
        public string SqlServerUser { get; set; }
        public string SqlServerPassword { get; set; }
        public string SapUserName { get; set; }
        public string DbServerType { get; set; }
        public string SapPassword { get; set; }
        public string SourceLocationForExcelFile { get; set; }
        public string TargetLocationForExcelFile { get; set; }
    }
}
