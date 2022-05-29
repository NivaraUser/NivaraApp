using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Data.Models
{
    public class TaskComments
    {
        public long Id { get; set; }
        public int TaskId { get; set; }
        public int? EmployeeId { get; set; }
        public int? CompanyId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
