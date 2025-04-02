using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Models
{
    public class Teacher : BaseModel
    {
        [Display(Name = "Имя")]
        public string? Name { get; set; }
        //[Display(Name = "Факультет")]
        //public int FacilityId { get; set; }
        //[ForeignKey("FacilityId")]
        //public virtual Facility? Facility { get; set; }

    }
}
