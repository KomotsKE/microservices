using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.DBContext;

namespace OrderService.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.Category.Name == category)
            .ToListAsync();
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Name == name);
    }
}