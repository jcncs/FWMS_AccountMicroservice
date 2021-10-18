using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Models
{
    public class User_Info
    {
        [Key]
        public string UserId { get; set; }
        public string Email { get; set; }
        public string HomePhone { get; set; }
        public string CreatedBY { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
