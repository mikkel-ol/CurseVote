using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Curse.Models
{
    public class User
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
        public string Character { get; set; }
        public int Prio1 { get; set; }
        public int Prio2 { get; set; }
        public int Prio3 { get; set; }
    }
}
