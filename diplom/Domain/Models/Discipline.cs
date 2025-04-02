using System.ComponentModel.DataAnnotations;
namespace Domain.Models
{
    public class Discipline : BaseModel
    {
        [Display(Name = "Дисциплина")]
        public string? Name { get; set; }

    }
}
