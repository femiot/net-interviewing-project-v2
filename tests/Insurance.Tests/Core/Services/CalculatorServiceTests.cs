using Insurance.Core.Interfaces;
using Insurance.Core.Services;
using Insurance.Shared.DTOs;
using Insurance.Shared.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.Core.Services
{
    public class CalculatorServiceTests
    {
        private readonly Mock<ILogger<CalculatorService>> _loggerMock;
        private readonly Mock<IProductIntegration> _productIntegrationMock;
        private readonly Mock<IProductTypeIntegration> _productTypeIntegrationMock;
        private readonly Mock<ISurchargeService> _surchargeServiceMock;
        private Mock<IInsuranceUnitOfWork> _insuranceUnitOfWorkMock;
        private readonly CalculatorService _serviceToTest;
        public CalculatorServiceTests()
        {
            _loggerMock = new Mock<ILogger<CalculatorService>>();
            _productIntegrationMock = new Mock<IProductIntegration>();
            _productTypeIntegrationMock = new Mock<IProductTypeIntegration>();
            _surchargeServiceMock = new Mock<ISurchargeService>();
            _insuranceUnitOfWorkMock = new Mock<IInsuranceUnitOfWork>();

            _serviceToTest = new CalculatorService(_loggerMock.Object,
                _productIntegrationMock.Object, _productTypeIntegrationMock.Object,
                _surchargeServiceMock.Object, _insuranceUnitOfWorkMock.Object);
        }

        [Fact]
        public async Task CalculateProductsInsuranceValueAsync_Given_Null_Products_Should_Throw_Exception()
        {
            _productIntegrationMock.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(default(List<ProductIntegrationDto>));

            await Assert.ThrowsAsync<Exception>(() => _serviceToTest.CalculateProductsInsuranceValueAsync(null));
        }

        [Fact]
        public async Task CalculateProductsInsuranceValueAsync_Given_Null_ProductTypes_Should_Throw_Exception()
        {
            _productIntegrationMock.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(new List<ProductIntegrationDto> 
            { new ProductIntegrationDto { Id =1, ProductTypeId = 1, SalesPrice = 500 } });

            _productTypeIntegrationMock.Setup(x => x.GetAllProductTypesAsync()).ReturnsAsync(default(List<ProductTypeIntegrationDto>));

            await Assert.ThrowsAsync<Exception>(() => _serviceToTest.CalculateProductsInsuranceValueAsync(null));
        }

        [Fact]
        public async Task GetExtraCostInsuranceValueAsync_Given_ApplyCostRule_ShouldReturnZero()
        {
           var productTypeIntegrationDto = new ProductTypeIntegrationDto { Name = "test" };
            _insuranceUnitOfWorkMock.Setup(x => x.Repository<InsuranceExtraCost>()
            .FirstOrDefaultAsync(x => productTypeIntegrationDto.Name == x.ProductName))
                .ReturnsAsync(new InsuranceExtraCost { ApplyCostRangeRule = true });

            var result = await _serviceToTest.GetExtraCostInsuranceValueAsync(productTypeIntegrationDto, 1, 0);
        }
    }
}