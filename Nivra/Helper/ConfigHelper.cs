using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nivara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Common.Helpers
{
    public class ConfigHelper
    {
        private readonly IOptions<AppSettingsModel> settings;

        public ConfigHelper(IOptions<AppSettingsModel> _settings)
        {
            settings = _settings;
           
        }
        public string MailHost
        {
            get
            {
                return settings.Value.MailHost;
            }
        }
        public string MailUserName
        {

            get
            {
                return settings.Value.MailUserName;
            }
        }
        public string MailFromName
        {
            get
            {
                return settings.Value.MailFromName;
            }
        }
        public string MailPassword
        {

            get
            {
                return settings.Value.MailPassword;
            }
        }
        public bool MailEnableSSL
        {

            get
            {
                return settings.Value.MailEnableSSL;
            }
        }
        public int MailPortNumber
        {

            get
            {
                return settings.Value.MailPortNumber;
            }
        }
    }
}
