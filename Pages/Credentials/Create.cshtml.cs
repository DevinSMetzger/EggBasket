using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EggBasket.Data;
using EggBasket.Models;
using static System.Threading.Thread;

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

        public IActionResult OnPostGenPass() {
			DateTime current;
			long time = 0;
			long seed = 0;
			bool valid = false;
			String genPass = "";
			int trials = 1;
			char[] filter = { '"', '#', '$', '%', '&', '^', '*', '(', ')', '-', '=', '+', ',', '\'', '.'
			,'/', ':', ';', '<', '>', '@', '[', ']', '\\', '_', '`'}; //Excluded characters

			for (int k = 0; k < trials; k++)
			{
				for (int j = 0; j < 15; j++)
				{
					while (!valid)
					{
						current = DateTime.Now;
						time = current.Ticks;
						if (time % 10 == 0)
						{
							time = time / 10; //Brings time value down to a more manageable size
						}
						seed = (time % 6967) * 97;  //Math to bring number down to values associated to ASCII characters
						seed = (seed % 90) + 33;

						valid = true;
						for (int i = 0; i < filter.Length; i++)
						{
							if ((char) seed == filter[i])
							{
								valid = false;
								Sleep(1);
							}
						}
					}

					//Console.WriteLine("Time = " + time);
					//Console.WriteLine("Seed = " + seed);
					//Console.WriteLine((char)seed);

					Sleep(10);
					genPass += (char)seed;
					valid = false;
				}
				ViewData["GeneratedPassword"] = genPass;
			}
			return Page();
        }
    }
}
