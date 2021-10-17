using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Models
{
    public class UserProfileView
    {
        [Key]
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string HomePhone { get; set; }
        public string OfficePhone { get; set; }
        public string Email { get; set; }
        public string IsAccountDisabled { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

    }
}
