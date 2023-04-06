using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Dtos
{
    public class EditUserDto
    {
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string Email { get; set; }
        [MaxLength(150)]
        public string Password { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}
