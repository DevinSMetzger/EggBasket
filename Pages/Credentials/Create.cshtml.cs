using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EggBasket.Data;
using EggBasket.Models;

namespace EggBasket.Pages.Credentials
{
    public class CreateModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;

        public CreateModel(EggBasket.Data.CredentialContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Credential Credential { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Credentials.Add(Credential);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
