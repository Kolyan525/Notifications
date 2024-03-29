﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Notifications.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace Notifications.BL.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>> expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<string> includes = null
        );
        Task<IPagedList<T>> GetAll(RequestParams requestParams, List<string> includes = null);
        Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null);
        Task<T> GetFirstOrDefault(Expression<Func<T, bool>> selector,
                                          Expression<Func<T, bool>> predicate = null,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        public Task<IList<T>> GetAllHere(Expression<Func<T, bool>> selector = null,
                                          Expression<Func<T, bool>> predicate = null,
                                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                          Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        bool Exists(object primaryKey);
        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(long id);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
    }
}
