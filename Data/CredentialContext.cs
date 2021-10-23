using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EggBasket.Models;

namespace EggBasket.Data
{
    public class CredentialContext : DbContext
    {
        public CredentialContext (DbContextOptions<CredentialContext> options)
            : base(options)
        {
        }

        public DbSet<Credential> Credentials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Credential>().ToTable("Credential");
        }
    }
}
