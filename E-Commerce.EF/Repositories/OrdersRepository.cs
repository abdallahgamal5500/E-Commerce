using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Interfaces;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.EF.Repositories
{
    public class OrdersRepository : BaseRepository<Order>, IOrdersRepository
    {
        private readonly ApplicationDbContext _context;
        public OrdersRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<EntitiesDto>> GetOrderDetailsById(int orderId, User userModel) 
            =>  await 
            (
                from order in _context.Orders
                join user in _context.Users
                on order.UserId equals user.Id
                join orderItems in _context.OrdersItems
                on order.Id equals orderItems.OrderId
                join product in _context.Products
                on orderItems.ProductId equals product.Id
                join category in _context.Categories
                on product.CategoryId equals category.Id
                where order.Id == orderId 
                && (userModel.Role.Equals(Roles.CUSTOMER) ?
                userModel.Id == order.UserId : true)
                select new EntitiesDto
                {
                    User = user,
                    Order = order,
                    OrderItem = orderItems,
                    Product = product,
                    Category = category,
                }).ToListAsync();
    }
}
