using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Helpers.Constants
{
    public static class OrdersStatuses
    {
        public static string PENDING { get; } = "Pending";
        public static string SHIPPED { get; } = "Shipped";
        public static string DELIVERED { get; } = "Delivered";
    }
}
