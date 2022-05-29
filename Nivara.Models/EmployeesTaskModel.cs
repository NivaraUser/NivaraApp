using System;
using System.Collections.Generic;
using System.Text;

namespace Nivara.Models
{
    public class EmployeesTaskModel
    {
        public EmployeesTaskModel()
        {
            Employees = new List<EmployeeModel>();
            UsersTasks = new List<Tasks>();
        }
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public int EmpId { get; set; }
        public string TaskName { get; set; }
        public int TaskId { get; set; }
        public string AspNetUserId { get; set; }
        public List<EmployeeModel> Employees { get; set; }
       // public LiEmployeeModel> Employees { get; set; }

        public List<Tasks> UsersTasks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class Tasks
    {

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
