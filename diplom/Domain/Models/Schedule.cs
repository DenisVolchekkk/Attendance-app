using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace Domain.Models
{
    public class Schedule : BaseModel
    {
        [Display(Name = "Начало")]
        [JsonConverter(typeof(TimeSpanConverter))]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan StartTime { get; set; }
        [Display(Name = "Конец")]
        [JsonConverter(typeof(TimeSpanConverter))]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public  TimeSpan EndTime { get; set; }
        [Display(Name = "День недели")]
        public DayOfWeek? DayOfWeek { get; set; }
        [Display(Name = "Группа")]
        public int GroupId { get; set; }
        [Display(Name = "Преподаватель")]
        public int TeacherId { get; set; }
        [Display(Name = "Дисциплина")]
        public int DisciplineId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }
        [ForeignKey("DisciplineId")]
        public virtual Discipline? Discipline { get; set; }
    }
}
