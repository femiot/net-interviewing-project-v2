namespace Insurance.Core.Interfaces
{
    public interface ICalculatorService
    {
        Task<float> CalculateProductInsuranceValueAsync(int productId);
        Task<float> CalculateProductsInsuranceValueAsync(int[] productIds);
    }
}
