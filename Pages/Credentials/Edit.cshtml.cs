using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggBasket.Data;
using EggBasket.Models;
using System.Text.Json;
using EncryptDecryptLib;
using CertificateManager;
using System.Security.Cryptography.X509Certificates;

namespace EggBasket.Pages.Credentials
{
    public class EditModel : PageModel
    {
        private readonly EggBasket.Data.CredentialContext _context;

        private readonly SymmetricEncryptDecrypt _symmetricEncryptDecrypt;
        private readonly AsymmetricEncryptDecrypt _asymmetricEncryptDecrypt;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ImportExportCertificate _importExportCertificate;

        public EditModel(EggBasket.Data.CredentialContext context, SymmetricEncryptDecrypt symmetricEncryptDecrypt,
        AsymmetricEncryptDecrypt asymmetricEncryptDecrypt,
        ApplicationDbContext applicationDbContext,
        ImportExportCertificate importExportCertificate)
        {
            _context = context;
            _symmetricEncryptDecrypt = symmetricEncryptDecrypt;
            _asymmetricEncryptDecrypt = asymmetricEncryptDecrypt;
            _applicationDbContext = applicationDbContext;
            _importExportCertificate = importExportCertificate;
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
            var cert = GetCertificateWithPrivateKeyForIdentity(Credential);
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


            if (Credential == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Credential).State = EntityState.Modified;

            var (Key, IVBase64) = _symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV();

            var encryptedText = _symmetricEncryptDecrypt.Encrypt(Credential.username, IVBase64, Key);

            var targetUserPublicCertificate = GetCertificateWithPublicKeyForIdentity(Credential.owneremail);

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



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CredentialExists(Credential.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool CredentialExists(int id)
        {
            return _context.Credentials.Any(e => e.ID == id);
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
        private X509Certificate2 GetCertificateWithPublicKeyForIdentity(string email)
        {
            var user = _applicationDbContext.Users.First(user => user.Email == email);
            var cert = _importExportCertificate.PemImportCertificate(user.PemPublicKey);
            return cert;
        }
    }
}
