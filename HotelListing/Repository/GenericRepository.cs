using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly DatabaseContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(DatabaseContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            // get all the records in that table of _db
            IQueryable<T> query = _db;

            // this is for filtering the data
            if (expression != null)
            {
                query = query.Where(expression);
            }

            // this is for including the other connected entities to this Entity like the Country in Hotel
            if (includes != null )
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            // this is for filtering the data
            if (expression != null)
            {
                query = query.Where(expression);
            }

            // this is for including the other connected entities to this Entity like the Country in Hotel
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.AsNoTracking()  // becase we are just getting the data from database and sending to client then no need to track
                .ToListAsync();
        }

        // ALWAYS put nullable parameters at the beginning of the parameter list 
        //public async Task<IPagedList<T>> GetAll(RequestParams requestParams = null, List<string> includes = null, )

        public async Task<IPagedList<T>> GetPagedList(RequestParams requestParams, List<string> includes = null)

        {
            IQueryable<T> query = _db;

            // this is for filtering the data
            //if (expression != null)
            //{
            //    query = query.Where(expression);
            //}

            // this is for including the other connected entities to this Entity like the Country in Hotel
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }


            /// Order by is not included here 

            //if (orderBy != null)
            //{
            //    query = orderBy(query);
            //}

            return await query.AsNoTracking()  // becase we are just getting the data from database and sending to client then no need to track
                //.ToListAsync();
                .ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);
        }


        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        
        }
    }
}
