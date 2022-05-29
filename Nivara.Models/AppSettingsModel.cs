using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class AppSettingsModel
    {
        public string MailHost { get; set; }
        public int MailPortNumber { get; set; }
        public bool MailEnableSSL { get; set; }
        public string MailPassword { get; set; }
        public string MailUserName { get; set; }
        public string MailFromName { get; set; }
    }
}
