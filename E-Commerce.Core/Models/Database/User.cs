using E_Commerce.Core.Helpers.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Database
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(30)]
        public string Email { get; set; }
        [MaxLength(150)]
        public string Password { get; set; }
        [MaxLength(20)]
        public string Role { get; set; } = Roles.ADMIN;
        public string ProfileImageUrl { get; set; } = null;
        public override string ToString()
        {
            return
                $"Id: {Id}\n" +
                $"Name: {Name}\n" +
                $"Email: {Email}\n" +
                $"Password: {Password}\n" +
                $"Role: {Role}\n" +
                $"ProfileImageUrl: {ProfileImageUrl}\n" +
                $"--------------------";
        }
    }
}
