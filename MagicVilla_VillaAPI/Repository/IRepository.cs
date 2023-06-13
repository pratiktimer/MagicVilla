﻿using MagicVilla_VillaAPI.Models;
using System.Linq.Expressions;

namespace MagicVilla_VillaAPI.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAsyncList(Expression<Func<T,bool>> filter);

        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);

        Task CreateAsync(T entity);

        Task RemoveAsync(T entity);

        Task SaveAsync();
    }
}