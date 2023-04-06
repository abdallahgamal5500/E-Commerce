using E_Commerce.Core.Interfaces;
using E_Commerce.Core.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace E_Commerce.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class 
    {
        protected ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<T> GetById(int id) =>
            await _context.Set<T>().FindAsync(id);
        
        public async Task<IEnumerable<T>> GetAll() =>
            await _context.Set<T>().ToListAsync();
            
        public async Task<IEnumerable<T>> GetAll(string[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> GetAll(Expression<Func<T, dynamic>> selector) =>
            await _context.Set<T>().Select(selector).ToListAsync();

        public async Task<T> FindSingle(Expression<Func<T, bool>> condition) =>
            await _context.Set<T>().SingleOrDefaultAsync(condition);

        public async Task<T> FindSingle(Expression<Func<T, bool>> condition, string[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);
            return await query.SingleOrDefaultAsync(condition);
        }
        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> condition) =>
            await _context.Set<T>().Where(condition).ToListAsync();

        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> condition, string[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.Where(condition).ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> FindAll(Expression<Func<T, bool>> condition, Expression<Func<T, dynamic>> selector) =>
            await _context.Set<T>().Where(condition).Select(selector).ToListAsync();

        public async void Add(T entity) =>
            await _context.Set<T>().AddAsync(entity);
        public async void AddRange(IEnumerable<T> entities) =>
            await _context.Set<T>().AddRangeAsync(entities);
  
        public void Update(T entity) =>
            _context.Update(entity);
        public void Delete(T entity) =>
            _context.Set<T>().Remove(entity);

        public void Delete(IEnumerable<T> entities) =>
            _context.Set<T>().RemoveRange(entities); 
    }
}
