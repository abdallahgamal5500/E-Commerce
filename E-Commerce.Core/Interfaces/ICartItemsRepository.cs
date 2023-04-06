﻿using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    public interface ICartItemsRepository : IBaseRepository<CartItem>
    {
        Task<IEnumerable<EntitiesDto>> GetCartItemsDetailsByUserId(User user);
    }
}
