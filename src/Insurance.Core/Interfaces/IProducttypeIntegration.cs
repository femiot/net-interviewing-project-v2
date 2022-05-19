using Insurance.Shared.DTOs;

namespace Insurance.Core.Interfaces
{
    public interface IProductTypeIntegration
    {
        Task<ProductTypeIntegrationDto?> GetProductTypeByIdAsync(int productTypeId);
    }
}
