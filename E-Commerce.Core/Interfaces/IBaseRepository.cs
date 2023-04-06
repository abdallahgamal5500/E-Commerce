using E_Commerce.Core.Models.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<T> GetById(int id);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(string[] includes);
        Task<IEnumerable<dynamic>> GetAll(Expression<Func<T, dynamic>> selector);
        Task<T> FindSingle(Expression<Func<T, bool>> condition);
        Task<T> FindSingle(Expression<Func<T, bool>> condition, string [] includes);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> condition);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> condition, string[] includes);
        Task<IEnumerable<dynamic>> FindAll(Expression<Func<T, bool>> condition, Expression<Func<T, dynamic>> selector);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
    }
}
