using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Data.Models
{
    public class Chat
    {
       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("UsersTask")]
        public int UsersTaskId { get; set; }
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }

        [Column(TypeName = "text")]
        public string Message { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool? IsDeleted { get; set; }
        public string DocumentPath { get; set; }
        public virtual UsersTask UsersTask { get; set; }

    }
}
