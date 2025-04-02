using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Group : BaseModel
    {
        [Display(Name = "Название")]
        public string? Name { get; set; }
        [Display(Name = "Староста")]
        public int ChiefId { get; set; }
        [Display(Name = "Факультет")]
        public int FacilityId { get; set; }
        [ForeignKey("ChiefId")]
        public virtual Chief? Chief { get; set; }
        [ForeignKey("FacilityId")]
        public virtual Facility? Facility { get; set; }

    }
}
