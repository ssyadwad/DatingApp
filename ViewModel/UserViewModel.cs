using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DatingWebApp.ViewModel
{
    public class UserViewModel
    {
        [Required]
        [StringLength(50,ErrorMessage ="UserName is not appropriate",MinimumLength =1)]
        public string UserName { get; set; }

        [Required]
        [StringLength(10,ErrorMessage ="You must specify password of 10 characters")]
        public string Password { get; set; }
    }
}
