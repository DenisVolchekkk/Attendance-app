using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class FacilityViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите Название")]
        [Display(Name = "Название")]
        public string? Name { get; set; }
    }
}
