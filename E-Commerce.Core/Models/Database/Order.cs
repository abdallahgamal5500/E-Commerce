using E_Commerce.Core.Helpers.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Models.Database
{
    public class Order
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        [MaxLength(150)]
        public string Address { get; set; }
        public decimal TotalCost { get; set; }
        [MaxLength(20)]
        public string Status { get; set; } = OrdersStatuses.PENDING;
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime DeliveryDate { get; set; } = DateTime.UtcNow.AddDays(3);
    }
}
