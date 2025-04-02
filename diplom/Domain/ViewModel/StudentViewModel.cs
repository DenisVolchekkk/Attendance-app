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
    public class StudentViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Введите группу")]
        [Display(Name = "Группа")]
        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }

    }
}
