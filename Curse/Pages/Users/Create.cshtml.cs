using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Curse.Models;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Curse.Pages.Users
{
    public class CreateModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public CreateModel(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (string.IsNullOrEmpty(User.Character))
            {
                ViewData["CharError"] = _configuration["Errors:CharEmpty"];
                return Page();
            }

            if (string.IsNullOrEmpty(User.Password))
            {
                ViewData["CharError"] = _configuration["Errors:PasswordEmpty"];
                return Page();
            }

            User.Character = User.Character.ToUpper();
            User.Password = Data.Encryption.PasswordHash.CreateHash(User.Password);
            var scrambler = new Data.DataScrambler();
            byte[] key = Encoding.ASCII.GetBytes(_configuration["EncryptionVariables:Key"]);
            byte[] IV = Encoding.ASCII.GetBytes(_configuration["EncryptionVariables:IV"]);
            //var encryptedEmail = scrambler.Encrypt(User.Email, key, IV);
            var encryptedCharacter = scrambler.Encrypt(User.Character, key, IV);
            //var bigStringEmail = BitConverter.ToString(encryptedEmail);
            var bigStringChar = BitConverter.ToString(encryptedCharacter);
            //User.Email = bigStringEmail;
            User.Character = bigStringChar;

            if (_context.Users.Any(x => x.Character.Equals(User.Character)))
            {
                ViewData["CharError"] = _configuration["Errors:CharExists"];
                return Page();
            }

            //if (_context.Users.Any(x => x.Character == User.Character))
            //{
            //    ViewData["CharError"] = "Character already exists";
            //    return Page();
            //}

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Login");
        }
    }
}
