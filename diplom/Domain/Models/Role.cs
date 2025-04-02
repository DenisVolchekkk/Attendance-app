using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace Domain.Models
{
    public  class Role : IdentityRole
    {
        public string Description { get; set; }
    }
}
