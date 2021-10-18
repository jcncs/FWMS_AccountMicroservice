using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Models
{
    public class User_Role
    {
        [Key]
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public Role role { get; set; }

    }
}
