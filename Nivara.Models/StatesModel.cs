using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nivara.Models
{
    public class StatesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key   
        [ForeignKey("Countries")]
        public int CounteryId { get; set; }
        public CountriesModel Countries { get; set; }
    }
}
