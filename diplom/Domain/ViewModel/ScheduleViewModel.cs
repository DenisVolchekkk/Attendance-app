using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class ScheduleViewModel : BaseModel
    {
        [Required(ErrorMessage = "Введите начальное время")]
        [Display(Name = "Начало")]
        [JsonConverter(typeof(TimeSpanConverter))]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan StartTime { get; set; }
        [Required(ErrorMessage = "Введите конечное время")]
        [Display(Name = "Конец")]
        [JsonConverter(typeof(TimeSpanConverter))]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan EndTime { get; set; }
        [Required(ErrorMessage = "Введите день недели")]
        [Display(Name = "День недели")]
        public DayOfWeek? DayOfWeek { get; set; }
        [Required(ErrorMessage = "Введите группу")]
        [Display(Name = "Группа")]
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Введите преподавателя")]
        [Display(Name = "Преподаватель")]
        public int TeacherId { get; set; }
        [Required(ErrorMessage = "Введите дисицпилину")]
        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }
        [Display(Name = "Дисциплина")]
        public int DisciplineId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }
        [ForeignKey("DisciplineId")]
        public virtual Discipline? Discipline { get; set; }

    }
}
