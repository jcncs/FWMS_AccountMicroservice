using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Models
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Disable { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string PwdHash { get; set; }

        public User_Info user_Info { get; set; }
    }
}
