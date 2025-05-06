using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class Chief : BaseModel
    {

        [Display(Name = "Имя")]
        public string? Name { get; set; }
        [Display(Name = "Группа")]
        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }

    }
}
