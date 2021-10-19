using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Models
{
    public class Role
    {
        [Key]
        public string RoleId { get; set;}

        public string RoleName { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get;set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }


    }
}
