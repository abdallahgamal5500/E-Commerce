using E_Commerce.Core;
using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Interfaces;
using E_Commerce.Core.Models.Database;
using E_Commerce.EF.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace E_Commerce.EF
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        public IBaseRepository<User> Users { get; private set; }
        public IBaseRepository<Product> Products { get; private set; }
        public IBaseRepository<Category> Categories { get; private set; }
        public ICartItemsRepository CartItems { get; private set; }
        public IOrdersRepository Orders { get; private set; }
        public IBaseRepository<OrderItem> OrdersItems { get; private set; }
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new BaseRepository<User>(context);
            Products = new BaseRepository<Product>(context);
            Categories = new BaseRepository<Category>(context);
            CartItems = new CartItemsRepository(context);
            Orders = new OrdersRepository(context);
            OrdersItems = new BaseRepository<OrderItem>(context);
        }
        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
