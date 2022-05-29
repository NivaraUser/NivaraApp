using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class PreDefinedTaskViewModel
    {
        public List<PreDefinedTaskModel> PreDefinedTasks { get; set; }
        public string UserName { get; set; }

        public List<PreDefinedTaskModel> PreDefinedNotificationTasks { get; set; }
    }
}
