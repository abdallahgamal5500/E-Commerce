using E_Commerce.Core.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Dtos
{
    public class EntitiesDto
    {
        public User User { get; set; }
        public Product Product { get; set; }
        public Category Category { get; set; }
        public CartItem CartItem { get; set; }
        public Order Order { get; set; }
        public OrderItem OrderItem { get; set; }
    }
}
