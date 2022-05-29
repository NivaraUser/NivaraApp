using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nivara.Data.Models
{
    public class UsersTask
    {
        public UsersTask()
        {
            this.UsersTaskDocument = new HashSet<UsersTaskDocument>();
            this.EmployeesTask = new HashSet<EmployeesTask>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Title { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? AssignedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int? CategeoryId { get; set; }
        public ICollection<UsersTaskDocument> UsersTaskDocument { get; set; }
        [ForeignKey("EndUsers")]
        public int? EndUsersId { get; set; }
        public virtual EndUsers EndUsers { get; set; }
        public ICollection<EmployeesTask> EmployeesTask { get; set; }


        [ForeignKey("Company")]
        public int? CompanyId   { get; set; }
        public virtual Company Company { get; set; }

        public string Remarks { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }

        [ForeignKey("TaskStatus")]
        public int TaskStatusId { get; set; }
        public virtual TaskStatus TaskStatus { get; set; }
        //  public bool IsRead { get; set; }

        #region  --Added By Nilasish for notificationwork on 19052022
        public int? IsAdminRead { get; set; }
        public int? IsClientRead { get; set; }
        public int? IsEmployeeRead { get; set; }

        #endregion --Added By Nilasish for notificationwork on 19052022

    }
}
