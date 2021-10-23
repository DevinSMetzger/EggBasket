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
    public class DeleteModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;

        public DeleteModel(EggBasket.Data.CredentialContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Credential Credential { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Credential = await _context.Credentials.FirstOrDefaultAsync(m => m.ID == id);

            if (Credential == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Credential = await _context.Credentials.FindAsync(id);

            if (Credential != null)
            {
                _context.Credentials.Remove(Credential);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
