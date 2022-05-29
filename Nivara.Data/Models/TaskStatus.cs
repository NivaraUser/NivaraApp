using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nivara.Data.Models
{
    public class TaskStatus
    {
        public TaskStatus()
        {
            this.UsersTasks = new HashSet<UsersTask>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UsersTask> UsersTasks { get; set; }
    }
}
