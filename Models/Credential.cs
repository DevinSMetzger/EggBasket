using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
namespace EggBasket.Models
{
    public class Credential
    {

        public int ID { get; set; }

        [Display(Name = "Username")]
        public String username { get; set; }
        [Display(Name = "Password")]
        public string password { get; set; }

        [Display(Name = "Secure Note")]
        public string secureNote { get; set; }


        public string roleID { get; set; }

        public string company { get; set; }

        [Required]
        [Display(Name = "Role access?")]
        public bool personal { get; set; }

        public string owneremail { get; set; }

    }
}
