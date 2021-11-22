using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EggBasket.Data;
using EggBasket.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using EncryptDecryptLib;
using CertificateManager;
using EggBasket.Areas.Identity.Data;
using System.Security.Cryptography.X509Certificates;

namespace EggBasket.Pages.Credentials
{
   
    public class DeleteModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SymmetricEncryptDecrypt _symmetricEncryptDecrypt;
        private readonly AsymmetricEncryptDecrypt _asymmetricEncryptDecrypt;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ImportExportCertificate _importExportCertificate;
        private readonly UserManager<EggBasketUser> _userManager;
        private readonly EggBasket.Data.ApplicationDbContext _usersContext;

        public DeleteModel(EggBasket.Data.CredentialContext context, SymmetricEncryptDecrypt symmetricEncryptDecrypt,
        AsymmetricEncryptDecrypt asymmetricEncryptDecrypt,
        ApplicationDbContext applicationDbContext,
        ImportExportCertificate importExportCertificate, RoleManager<IdentityRole> roleManager, UserManager<EggBasketUser> userManager, EggBasket.Data.ApplicationDbContext usersContext)
        {
            _context = context;
            _symmetricEncryptDecrypt = symmetricEncryptDecrypt;
            _asymmetricEncryptDecrypt = asymmetricEncryptDecrypt;
            _applicationDbContext = applicationDbContext;
            _importExportCertificate = importExportCertificate;
            _userManager = userManager;
            _roleManager = roleManager;
            _usersContext = usersContext;
        }

        public Credential Credential { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Credential = await _context.Credentials.FirstOrDefaultAsync(m => m.ID == id);
            var cert = GetCertificateWithPrivateKeyForIdentity(Credential);

            if (Credential == null)
            {
                return NotFound();
            }
            var encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(Credential.username);

            var key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                Utils.CreateRsaPrivateKey(cert));

            var IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                Utils.CreateRsaPrivateKey(cert));

            Credential.username = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

            encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(Credential.password);

            key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                Utils.CreateRsaPrivateKey(cert));

            IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                Utils.CreateRsaPrivateKey(cert));

            Credential.password = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

            encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(Credential.secureNote);

            key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                Utils.CreateRsaPrivateKey(cert));

            IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                Utils.CreateRsaPrivateKey(cert));

            Credential.secureNote = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);
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
        private X509Certificate2 GetCertificateWithPrivateKeyForIdentity(Credential item)
        {
            var user = _applicationDbContext.Users.First(user => user.Email == item.owneremail);

            var certWithPublicKey = _importExportCertificate.PemImportCertificate(user.PemPublicKey);
            var privateKey = _importExportCertificate.PemImportPrivateKey(user.PemPrivateKey);

            var cert = _importExportCertificate.CreateCertificateWithPrivateKey(
                certWithPublicKey, privateKey);

            return cert;
        }
    }
}
