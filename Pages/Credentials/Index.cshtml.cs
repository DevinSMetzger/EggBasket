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
using CertificateManager;
using EncryptDecryptLib;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using EggBasket.Areas.Identity.Data;

namespace EggBasket.Pages.Credentials
{
    public class IndexModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SymmetricEncryptDecrypt _symmetricEncryptDecrypt;
        private readonly AsymmetricEncryptDecrypt _asymmetricEncryptDecrypt;

        private readonly ImportExportCertificate _importExportCertificate;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<EggBasketUser> _userManager;
        private readonly EggBasket.Data.ApplicationDbContext _usersContext;

        public IndexModel(EggBasket.Data.CredentialContext context, SymmetricEncryptDecrypt symmetricEncryptDecrypt,
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

        public IList<Credential> Credential { get;set; }


        public async Task OnGetAsync()
        {
            
            Credential = await _context.Credentials.ToListAsync();
            if (Credential.Count > 0) {
                foreach (Credential item in _context.Credentials.ToList())
                {
                    var cert = GetCertificateWithPrivateKeyForIdentity(item);
                    var userResult = _userManager.FindByEmailAsync(User.Identity.Name);
                    string role = _usersContext.UserRoles.Where(r => r.UserId == userResult.Result.Id).First().RoleId;
                    Credential.Remove(item);
                    if (item.personal == false)
                    {
                        if (item.roleID == role && item.company == userResult.Result.CompanyName)
                        {
                            var encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(item.username);

                            var key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                                Utils.CreateRsaPrivateKey(cert));

                            var IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                                Utils.CreateRsaPrivateKey(cert));

                            item.username = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

                            encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(item.password);

                            key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                                Utils.CreateRsaPrivateKey(cert));

                            IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                                Utils.CreateRsaPrivateKey(cert));

                            item.password = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

                            encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(item.secureNote);

                            key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                                Utils.CreateRsaPrivateKey(cert));

                            IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                                Utils.CreateRsaPrivateKey(cert));

                            item.secureNote = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

                            Credential.Add(item);
                        }
                    }
                    else
                    {
                        if(item != null)
                        {
                            var userIDmatches = _context.CredentialAccess.Where(r => r.userid == userResult.Result.Id);

                            if (userIDmatches.Any(t => t.credential == item.ID))
                            {

                                var encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(item.username);

                                var key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                                    Utils.CreateRsaPrivateKey(cert));

                                var IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                                    Utils.CreateRsaPrivateKey(cert));

                                item.username = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

                                encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(item.password);

                                key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                                    Utils.CreateRsaPrivateKey(cert));

                                IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                                    Utils.CreateRsaPrivateKey(cert));

                                item.password = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

                                encryptedDto = JsonSerializer.Deserialize<EncryptedDto>(item.secureNote);

                                key = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.Key,
                                    Utils.CreateRsaPrivateKey(cert));

                                IV = _asymmetricEncryptDecrypt.Decrypt(encryptedDto.IV,
                                    Utils.CreateRsaPrivateKey(cert));

                                item.secureNote = _symmetricEncryptDecrypt.Decrypt(encryptedDto.EncryptedText, IV, key);

                                Credential.Add(item);
                            }
                        }
                        

                    }

                }

            } 
        }


        public IActionResult OnPost()
        {

            var emailAddress = Request.Form["email"];
            var credentialID = Request.Form["credentialID"];
            EggBasketUser user = _userManager.FindByEmailAsync(emailAddress).Result;

            CredentialAccess access = new CredentialAccess();
            access.credential = Int32.Parse(credentialID);
            access.userid = user.Id;
            _context.CredentialAccess.Add(access);



            _context.SaveChanges();
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
