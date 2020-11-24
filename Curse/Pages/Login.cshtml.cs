using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Curse.Models;
using Microsoft.AspNetCore.Http;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Curse.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public LoginModel(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [BindProperty]
        public User User { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (string.IsNullOrEmpty(User.Password) || string.IsNullOrEmpty(User.Character))
            {
                ViewData["LoginError"] = _configuration["Errors:LoginError"];
                return Page();
            }
            User.Character = User.Character.ToUpper();
            var scrambler = new Data.DataScrambler();
            byte[] key = Encoding.ASCII.GetBytes(_configuration["EncryptionVariables:Key"]);
            byte[] IV = Encoding.ASCII.GetBytes(_configuration["EncryptionVariables:IV"]);

            //var encryptedEmail = scrambler.Encrypt(User.Character, key, IV);
            //var bigStringEmail = BitConverter.ToString(encryptedEmail);
            var encryptedChar = scrambler.Encrypt(User.Character, key, IV);
            var stringChar = BitConverter.ToString(encryptedChar);

            var user = _context.Users.SingleOrDefault(x => x.Character.Equals(stringChar));
            if (user != null)
            {
                if (Data.Encryption.PasswordHash.ValidatePassword(User.Password, user.Password))
                {
                    HttpContext.Session.SetInt32("_Id", user.Id);
                    return RedirectToPage("/Vote");
                }
                else
                {
                    ViewData["LoginError"] = _configuration["Errors:LoginError"];
                    return Page();
                }
            }
            else
            {
                ViewData["LoginError"] = _configuration["Errors:LoginError"];
                return Page();
            }
        }
    }
}
