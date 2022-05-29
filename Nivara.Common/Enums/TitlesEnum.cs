using System.ComponentModel.DataAnnotations;

namespace Nivara.Common.Enums
{
    public enum TitlesEnum
    {
        [Display(Name = "Mr.")]
        Mr,
        [Display(Name = "Mrs.")]
        Mrs,
        [Display(Name = "Ms.")]
        Ms,
        [Display(Name = "Miss")]
        Miss
    }
}
