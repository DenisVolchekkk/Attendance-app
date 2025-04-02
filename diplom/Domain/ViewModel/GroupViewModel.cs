using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class GroupViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите группу")]
        [Display(Name = "Название")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Введите Старосту")]
        [Display(Name = "Староста")]
        public int ChiefId { get; set; }
        [Required(ErrorMessage = "Введите факультет")]
        [Display(Name = "Факультет")]
        public int FacilityId { get; set; }
        [ForeignKey("ChiefId")]
        public virtual Chief? Chief { get; set; }
        [ForeignKey("FacilityId")]
        public virtual Facility? Facility { get; set; }

    }
}
