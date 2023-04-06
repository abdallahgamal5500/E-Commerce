using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Dtos
{
    public class LoginDto
    {
        [MaxLength(30)]
        public string Email { get; set; }
        [MaxLength(150)]
        public string Password { get; set; }
    }
}
