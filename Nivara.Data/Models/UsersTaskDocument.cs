using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Nivara.Data.Models
{
    public class UsersTaskDocument
    {
       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPath { get; set; }
        public int UsersTaskId { get; set; }
        public virtual UsersTask UsersTask { get; set; }

    }
}
