using E_Commerce.Core.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Dtos
{
    public class ProductDto
    {
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string imageUrl { get; set; }
        public int CategoryId { get; set; }
        [MaxLength(20)]
        public string Color { get; set; }
        [MaxLength(20)]
        public string Size { get; set; }
    }
}
