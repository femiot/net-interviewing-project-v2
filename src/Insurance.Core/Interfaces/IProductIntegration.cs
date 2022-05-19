using Insurance.Shared.DTOs;

namespace Insurance.Core.Interfaces
{
    public interface IProductIntegration
    {
        Task<ProductIntegrationDto?> GetProductByIdAsync(int productId);
        Task<List<ProductIntegrationDto>?> GetAllProductsAsync();
    }
}
