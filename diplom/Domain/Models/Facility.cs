
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Facility : BaseModel
    {
        [Display(Name = "Название")]
        public string? Name { get; set; }
    }
}
