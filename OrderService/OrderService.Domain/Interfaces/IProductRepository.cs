using CoreLib.Interfaces;
using OrderService.Domain.Entities;
namespace OrderService.Domain.Interfaces;


public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<Product?> GetByNameAsync(string name);
    }