using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Data.Models
{
    public class Cities
    {
        public Cities()
        {
            this.EndUsers = new HashSet<EndUsers>();
            this.Employee=new HashSet<Employee>();
            this.Company = new HashSet<Company>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("States")]
        public int StateId { get; set; }
        public virtual States States { get; set; }
        public ICollection<EndUsers> EndUsers { get; set; }
        public ICollection<Employee> Employee { get; set; }
        public ICollection<Company> Company { get; set; }
    }
}
