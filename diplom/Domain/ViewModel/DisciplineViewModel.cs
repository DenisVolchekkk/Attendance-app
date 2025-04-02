using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class DisciplineViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите название")]
        [Display(Name = "Дисциплина")]
        public string? Name { get; set; }

    }
}
