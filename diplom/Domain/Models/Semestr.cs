using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Semestr : BaseModel
    {
        [Required]
        [Display(Name = "Преподаватель")]
        public string Name { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
