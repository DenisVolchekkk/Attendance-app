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
    public class TeacherViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Введите факультет")]
        [Display(Name = "Факультет")]
        public int FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public virtual Facility? Facility { get; set; }

    }
}
