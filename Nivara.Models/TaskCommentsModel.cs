using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class TaskCommentsModel
    {
        public long Id { get; set; }
        public int TaskId { get; set; }
        public int? EmployeeId { get; set; }
        public int? CompanyId { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string EmployeeName { get; set; }
        public string CompanyName { get; set; }
    }
}
