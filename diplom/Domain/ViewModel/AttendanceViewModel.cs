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
    public class AttendanceViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите студента")]
        [Display(Name = "Студент")]
        public int StudentId { get; set; }
        [Required(ErrorMessage = "Введите расписание")]
        [Display(Name = "Расписание")]
        public int ScheduleId { get; set; }
        [Required(ErrorMessage = "Введите дату")]
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime? AttendanceDate { get; set; }
        [Required(ErrorMessage = "Введите присутствие")]
        [Display(Name = "Присутствовал")]
        public bool IsPresent { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual Schedule? Schedule { get; set; }
    }
}
