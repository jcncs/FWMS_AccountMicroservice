using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Models
{
    public class User_Role
    {
        public User User { get; set; }
        public Role Role { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? UpdateBy { get; set; }
         public DateTime? UpdateDate { get; set; }

    }
}
