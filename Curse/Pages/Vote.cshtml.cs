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
    public class VoteModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;

        public VoteModel(DatabaseContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [BindProperty]
        public User User { get; set; }
        [BindProperty]
        public List<Nominee> Nominees { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var scrambler = new Data.DataScrambler();
            id = HttpContext.Session.GetInt32("_Id");
            if (id == null)
            {
                return NotFound();
            }

            User = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            Nominees = _context.Nominees.ToList();

            byte[] key = Encoding.ASCII.GetBytes("HR$2pIjHR$2pIj12");
            byte[] IV = Encoding.ASCII.GetBytes("HR$2pIjHR$2pIj12");
            byte[] charDecrypt = new byte[(User.Character.Length + 1) / 3];
            for (int i = 0; i < charDecrypt.Length; i++)
            {
                charDecrypt[i] = (byte)(
                   "0123456789ABCDEF".IndexOf(User.Character[i * 3]) * 16 +
                   "0123456789ABCDEF".IndexOf(User.Character[i * 3 + 1])
                );
            }

            var decryptedChar = scrambler.Decrypt(charDecrypt, key, IV);

            if (!Data.CurseMembers.Members.Any(x => x.Contains(decryptedChar, StringComparison.OrdinalIgnoreCase)))
            {
                HttpContext.Session.SetString("vote", _configuration["Errors:AllowedToVote"]);
                return RedirectToPage("/Index");
            }

            if (User == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int Prio1Listbox, int Prio2Listbox, int Prio3Listbox)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var id = HttpContext.Session.GetInt32("_Id");
            if (id == null)
            {
                return NotFound();
            }
            if (Prio1Listbox == Prio2Listbox || Prio2Listbox == Prio3Listbox || Prio2Listbox == Prio3Listbox)
            {
                HttpContext.Session.SetString("vote", _configuration["Errors:MultiVoteError"]);
                return RedirectToPage("/Index");
            }

            User = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            User.Prio1 = _context.Nominees.SingleOrDefault(x => x.Id == Prio1Listbox).Id;
            User.Prio2 = _context.Nominees.SingleOrDefault(x => x.Id == Prio2Listbox).Id;
            User.Prio3 = _context.Nominees.SingleOrDefault(x => x.Id == Prio3Listbox).Id;

            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("vote", _configuration["Errors:SubmittedVote"]);
            return RedirectToPage("/Index");
        }
    }
}
