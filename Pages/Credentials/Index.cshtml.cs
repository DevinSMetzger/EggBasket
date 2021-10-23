using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EggBasket.Data;
using EggBasket.Models;

namespace EggBasket.Pages.Credentials
{
    public class IndexModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;

        public IndexModel(EggBasket.Data.CredentialContext context)
        {
            _context = context;
        }

        public IList<Credential> Credential { get;set; }

        public async Task OnGetAsync()
        {
            Credential = await _context.Credentials.ToListAsync();
        }
    }
}
