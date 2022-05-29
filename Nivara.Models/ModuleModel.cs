using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class ModuleModel
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string Url { get; set; }
        public string IconName { get; set; }
        public int? ParentModuleId { get; set; }
        public bool IsAssign { get; set; }
    }
}
