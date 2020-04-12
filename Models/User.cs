using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWebApp.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public byte[] passwordHash { get; set; }

        public byte[] salt { get; set; }
    }
}
