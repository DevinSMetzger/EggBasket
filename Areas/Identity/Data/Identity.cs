using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EggBasket.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EggBasket.Data
{
    public class Identity : IdentityDbContext<EggBasketUser>
    {
        public Identity(DbContextOptions<Identity> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
