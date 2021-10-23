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

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

		public void OnPost()
        {
			PassGen();
        }
        public void PassGen()
        {
			DateTime current;
			long time = 0;
			long seed = 0;
			bool valid = false;
			String genPass = "";
			int trials = 1;
			int[] filter = { 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 91, 92, 93, 94, 95, 96, 58, 59, 60, 61, 62, 64 }; //Excluded characters

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
							if (seed == filter[i])
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
		}
    }
}
