using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EggBasket.Data;
using EggBasket.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EggBasket.Pages.Credentials
{
    public class IndexModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(EggBasket.Data.CredentialContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public IList<Credential> Credential { get;set; }

        
        public async Task OnGetAsync()
        {
            Credential = await _context.Credentials.ToListAsync();

        }
    }
}
