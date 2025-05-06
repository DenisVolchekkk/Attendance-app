using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ChiefViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string? Name { get; set; }
        [Display(Name = "Группа")]
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }

    }
}
