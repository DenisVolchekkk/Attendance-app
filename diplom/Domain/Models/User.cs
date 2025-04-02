using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace Domain.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }

    }
}
