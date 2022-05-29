using Microsoft.AspNetCore.Http;
using Microsoft.Web.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Nivara.Models
{
    public class UsersTaskModel
    {
        public UsersTaskModel()
        {
            UsersTaskDocuments = new List<UsersTaskDocumentModel>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? AssignedDate { get; set; }

        public int? CategeoryId { get; set; }
        public List<UsersTaskDocumentModel> UsersTaskDocuments { get; set; }

        public string ProfileImagePath { get; set; }

        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public string UserName { get; set; }

        public List<IFormFile> postedFiles { get; set; }
        public int? EndUserId { get; set; }
        //public int? CompanyId { get; set; }
        public bool IsDeleted { get; set; }

        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string Remarks { get; set; }
        public string EmployeeAspNetUserId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public String StartDateFormatted { get { return String.Format("{0:MM/dd/yyyy}", StartDate); } }
        public String DueDateFormatted { get { return String.Format("{0:MM/dd/yyyy}", DueDate); } }
        //[Required]
        //[Display(Name = "File")]
        

        public IFormFile file { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<CompanyModel> Companies { get; set; }
        public int EmployeeId { get; set; }
        public List<EmployeeModel> Employees { get; set; }
        public int TaskStatusId { get; set; }

        public string TaskStatusName { get; set; }
        public List<TaskStatusModel> TaskStatus { get; set; }

        public TaskCommentsModel TaskComment { get; set; }
        public List<TaskCommentsModel> TaskComments { get; set; }

        public int AssignedEmployeeId { get; set; }
        public string AssignedEmployeeName { get; set; }
        public bool IsRead { get; set; }


        #region  --Added By Nilasish for notificationwork on 19052022
        public int IsAdminRead { get; set; }
        public int IsClientRead { get; set; }
        public int IsEmployeeRead { get; set; }

        #endregion --Added By Nilasish for notificationwork on 19052022



    }
}
