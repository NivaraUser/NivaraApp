using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Data.Models
{
    public class States
    {
        public States()
        {
            this.Cities = new HashSet<Cities>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Countries")]
        public int CounteryId { get; set; }
        public virtual Countries Countries { get; set; }

        public ICollection<Cities> Cities { get; set; }
    }
}
