using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EggBasket.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the EggBasketUser class
    public class EggBasketUser : IdentityUser
    {
        [PersonalData]
        public string CompanyName { get; set; }

    }
}
