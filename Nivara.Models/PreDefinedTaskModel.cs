using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class PreDefinedTaskModel
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string ServiceDescription { get; set; }
        public string LogoName { get; set; }
        public string ETD { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifyBy { get; set; }
        public List<IFormFile> PostedFiles { get; set; }

        #region  --Added By Nilasish for notificationwork on 19052022
        public int IsAdminRead { get; set; }
        public int IsClientRead { get; set; }
        public int IsEmployeeRead { get; set; }

        #endregion --Added By Nilasish for notificationwork on 19052022

    }
}
