using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Chief : BaseModel
    {

        [Display(Name = "Имя")]
        public string? Name { get; set; }

    }
}
