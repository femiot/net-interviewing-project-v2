namespace Insurance.Core.Interfaces
{
    public interface ICalculatorService
    {
        Task<float> CalculateProductInsuranceValueAsync(int productId);
    }
}
