using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Dtos
{
    public class OrderDto
    {
        [MaxLength(150)]
        public string Address { get; set; }
        public decimal TotalCost { get; set; }
    }
}
