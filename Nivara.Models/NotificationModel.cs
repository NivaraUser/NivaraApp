using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string ServiceDescription { get; set; }
        public string LogoName { get; set; }
        public string ETD { get; set; }
        public string ControllerName { get; set; }     
        public int IsAdminRead { get; set; }
        public int IsClientRead { get; set; }
        public int IsEmployeeRead { get; set; }

      

    }
}
