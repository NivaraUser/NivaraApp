using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nivara.Data.Models
{
    public class EmployeesTask
    {
       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
       
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifyBy { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [ForeignKey("UsersTask")]
        public int TaskId { get; set; }
        public virtual UsersTask UsersTask { get; set; }

       

        //
        // public ICollection<UsersTask> UsersTasks { get; set; }


        //public virtual EndUsers EndUsers { get; set; }
    }
}
