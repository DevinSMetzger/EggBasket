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
using Microsoft.AspNetCore.Identity;
using EggBasket.Areas.Identity.Data;
using EncryptDecryptLib;
using CertificateManager;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;

namespace EggBasket.Pages.Credentials
{
    public class CreateModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;
		private readonly UserManager<EggBasketUser> _userManager;
		private readonly EggBasket.Data.ApplicationDbContext _usersContext;
		private readonly SymmetricEncryptDecrypt _symmetricEncryptDecrypt;
		private readonly AsymmetricEncryptDecrypt _asymmetricEncryptDecrypt;
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly ImportExportCertificate _importExportCertificate;
		public CreateModel(EggBasket.Data.CredentialContext context,
			UserManager<EggBasketUser> userManager, EggBasket.Data.ApplicationDbContext usersContext, SymmetricEncryptDecrypt symmetricEncryptDecrypt,
		AsymmetricEncryptDecrypt asymmetricEncryptDecrypt,
		ApplicationDbContext applicationDbContext,
		ImportExportCertificate importExportCertificate)
        {
			_userManager = userManager;
            _context = context;
			_usersContext = usersContext;
			_symmetricEncryptDecrypt = symmetricEncryptDecrypt;
			_asymmetricEncryptDecrypt = asymmetricEncryptDecrypt;
			_applicationDbContext = applicationDbContext;
			_importExportCertificate = importExportCertificate;
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
			var result = _userManager.FindByEmailAsync(User.Identity.Name);

			var role = _usersContext.UserRoles.Where(r => r.UserId == result.Result.Id);

			Credential.roleID = role.First().RoleId;

			Credential.userId = result.Result.Id;
			Credential.company = result.Result.CompanyName;

			var (Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();

			var encryptedText = _symmetricEncryptDecrypt.Encrypt(Credential.username, IVBase64, Key);

			var targetUserPublicCertificate = GetCertificateWithPublicKeyForIdentity(User.Identity.Name);

			var encryptedKey = _asymmetricEncryptDecrypt.Encrypt(Key,
				Utils.CreateRsaPublicKey(targetUserPublicCertificate));

			var encryptedIV = _asymmetricEncryptDecrypt.Encrypt(IVBase64,
				Utils.CreateRsaPublicKey(targetUserPublicCertificate));

			var encryptedDto = new EncryptedDto
			{
				EncryptedText = encryptedText,
				Key = encryptedKey,
				IV = encryptedIV
			};

			Credential.username = JsonSerializer.Serialize(encryptedDto);

			(Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();

			encryptedText = _symmetricEncryptDecrypt.Encrypt(Credential.password, IVBase64, Key);

			targetUserPublicCertificate = GetCertificateWithPublicKeyForIdentity(User.Identity.Name);

			encryptedKey = _asymmetricEncryptDecrypt.Encrypt(Key,
				Utils.CreateRsaPublicKey(targetUserPublicCertificate));

			encryptedIV = _asymmetricEncryptDecrypt.Encrypt(IVBase64,
				Utils.CreateRsaPublicKey(targetUserPublicCertificate));

			encryptedDto = new EncryptedDto
			{
				EncryptedText = encryptedText,
				Key = encryptedKey,
				IV = encryptedIV
			};
			Credential.password = JsonSerializer.Serialize(encryptedDto);


			(Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();

			encryptedText = _symmetricEncryptDecrypt.Encrypt(Credential.secureNote, IVBase64, Key);

			targetUserPublicCertificate = GetCertificateWithPublicKeyForIdentity(User.Identity.Name);

			encryptedKey = _asymmetricEncryptDecrypt.Encrypt(Key,
				Utils.CreateRsaPublicKey(targetUserPublicCertificate));

			encryptedIV = _asymmetricEncryptDecrypt.Encrypt(IVBase64,
				Utils.CreateRsaPublicKey(targetUserPublicCertificate));

			encryptedDto = new EncryptedDto
			{
				EncryptedText = encryptedText,
				Key = encryptedKey,
				IV = encryptedIV
			};

			Credential.secureNote = JsonSerializer.Serialize(encryptedDto);

			_context.Credentials.Add(Credential);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
		private X509Certificate2 GetCertificateWithPublicKeyForIdentity(string email)
		{
			var user = _applicationDbContext.Users.First(user => user.Email == email);
			var cert = _importExportCertificate.PemImportCertificate(user.PemPublicKey);
			return cert;
		}

		public IActionResult OnPostGenPass() {
			DateTime current;
			long time = 0;
			long seed = 67; //change to account number, current number is temporary
			bool valid = false;
			String genPass = "";
			//int trials = 1;
			char[] filter = { '"', '#', '$', '%', '&', '^', '*', '(', ')', '-', '=', '+', ',', '\'', '.'
			,'/', ':', ';', '<', '>', '@', '[', ']', '\\', '_', '`'}; //Excluded characters
			DateTime forceSpawn = DateTime.Now;
			int forceChar = (int) forceSpawn.Ticks % 15; 
			int userName = 0;
			for (int i = 0; i < User.Identity.Name.Length; i++)
			{
				userName = userName + User.Identity.Name[i];
			}
			Console.WriteLine(userName);
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
					seed = (time % 6967) * seed;  //Math to bring number down to values associated to ASCII characters
					seed = seed / userName;
					seed = (seed % 90) + 33;

					valid = true;
					if (forceChar == j)
					{
						seed = (int)'!';
					}
					else
					{
						for (int i = 0; i < filter.Length; i++)
						{
							if ((char)seed == filter[i])
							{
								valid = false;
								Sleep(1);
							}
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
			return Page();
        }
    }
}
