using E_Commerce.Core.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Dtos
{
    public class ValidatingUserTokenDto
    {
        public int StatusCode { get; set; }
        public User User { get; set; }
        public string Token { get; set; }
        public bool IsTokenChanged { get; set; }
    }
}
