using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        public List<Role> RoleList { get; set; }
        public List<string> Roles { get; set; }
    }

}
