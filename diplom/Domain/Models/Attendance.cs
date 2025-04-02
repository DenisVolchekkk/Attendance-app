using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Attendance : BaseModel
    {
        [Display(Name = "Студент")]
        public int StudentId { get; set; }
        [Display(Name = "Расписание")]
        public int ScheduleId { get; set; }
        [Display(Name = "Дата")]
        [DataType(DataType.Date)]
        public DateTime? AttendanceDate { get; set; }
        [Display(Name = "Присутствовал")]
        public bool IsPresent { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual Schedule? Schedule { get; set; }
    }
}
