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
        public String username { get; set; }
        public string password { get; set; }

        public string secureNote { get; set; }

       public string userId { get; set; }

    }
}
