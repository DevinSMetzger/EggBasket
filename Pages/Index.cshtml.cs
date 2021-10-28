using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Threading.Thread; 
namespace EggBasket.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IndexModel(ILogger<IndexModel> logger, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task OnGetAsync()
        {

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await _roleManager.RoleExistsAsync("IT"))
            {
                await _roleManager.CreateAsync(new IdentityRole("IT"));
            }
            if (!await _roleManager.RoleExistsAsync("Marketing"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Marketing"));
            }
            if (!await _roleManager.RoleExistsAsync("Accounting"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Accounting"));
            }
            if (!await _roleManager.RoleExistsAsync("Legal"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Legal"));
            }
            if (!await _roleManager.RoleExistsAsync("HR"))
            {
                await _roleManager.CreateAsync(new IdentityRole("HR"));
            }
        }
    }
}
