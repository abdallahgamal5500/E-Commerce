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
    public class CartItemsRepository : BaseRepository<CartItem>, ICartItemsRepository
    {
        private readonly ApplicationDbContext _context;
        public CartItemsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<EntitiesDto>> GetCartItemsDetailsByUserId(User user)
            => await (
                from cartItem in _context.CartsItems
                join product in _context.Products
                on cartItem.ProductId equals product.Id
                join category in _context.Categories
                on product.CategoryId equals category.Id
                where cartItem.UserId == user.Id
                select new EntitiesDto
                {
                    User = user,
                    CartItem = cartItem,
                    Product = product,
                    Category = category,
                }).ToListAsync();
    }
}
