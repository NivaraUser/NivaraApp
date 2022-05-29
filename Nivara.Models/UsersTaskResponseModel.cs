using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class UsersTaskResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int TaskStatusId { get; set; }
        public string TaskStatusName { get; set; }
        public string Remarks { get; set; }
    }
}
