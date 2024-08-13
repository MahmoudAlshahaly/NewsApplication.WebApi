using Microsoft.EntityFrameworkCore;
using NewsApplication.DAL.DbContext;
using System.Linq.Expressions;

namespace NewsApplication.DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{


    private readonly NewsContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(NewsContext context )
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<IQueryable<T>> GetAllAsync()
    {
        return _dbSet.AsQueryable();
    }
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);
        _dbSet.Remove(entity);
       await _context.SaveChangesAsync();
    }
}
