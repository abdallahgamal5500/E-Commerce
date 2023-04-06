using E_Commerce.Core.Interfaces;
using E_Commerce.Core.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<User> Users { get; }
        IBaseRepository<Product> Products { get; }
        IBaseRepository<Category> Categories { get; }
        ICartItemsRepository CartItems { get; }
        IOrdersRepository Orders { get; }
        IBaseRepository<OrderItem> OrdersItems { get; }
        Task<int> Complete();
    }
}
