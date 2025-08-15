using exercise.wwwapi.Data;
using exercise.wwwapi.DTOs;
using exercise.wwwapi.Models;
using Microsoft.EntityFrameworkCore;

namespace exercise.wwwapi.Repository
{
    public class Repository : IRepository
    {
        private DataContext _db;
        public Repository(DataContext db)
        {
            _db = db;
        }
        public async Task<List<Product>> GetAsync()
        {
            return await _db.Products.ToListAsync();
        }

        public async Task<Product> AddAsync(Product entity)
        {   
            await _db.Products.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _db.Products.FindAsync(id);
        }

        public async Task<Product> DeleteAsync(int id)
        {
            var target = await _db.Products.FindAsync(id);
            _db.Products.Remove(target);
            await _db.SaveChangesAsync();
            return target;
        }

        public async Task<Product> UpdateAsync(int id, Product entity)
        {
            var target = await _db.Products.FindAsync(id);
            target = entity;
            await _db.SaveChangesAsync();
            return target;
        }
    }
}
