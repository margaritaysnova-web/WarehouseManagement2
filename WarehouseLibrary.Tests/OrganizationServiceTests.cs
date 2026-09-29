using System.Linq;
using System.Numerics;
using WarehouseData.Models;
using WarehouseLibrary.Services;
using Xunit;

namespace WarehouseLibrary.Tests
{
    public class OrganizationServiceTests
    {
        private readonly IOrganizationService _service;

        public OrganizationServiceTests()
        {
            _service = new OrganizationService();
        }

        [Fact]
        public void SeedTestData_ShouldAddOrganizations()
        {
            _service.SeedTestData();
            var orgs = _service.GetAll();

            Assert.NotEmpty(orgs);
            Assert.Equal(3, orgs.Count);
        }

        [Fact]
        public void Create_ValidName_ShouldCreateWithBigIntegerId()
        {
            var org = _service.Create("Тестовая организация");

            Assert.NotNull(org);
            Assert.Equal("Тестовая организация", org.OrgName);
            Assert.True(org.OrgId > BigInteger.Zero);
        }

        [Fact]
        public void GetById_ExistingOrganization_ShouldReturnIt()
        {
            _service.SeedTestData();
            var firstOrg = _service.GetAll().First();

            var found = _service.GetById(firstOrg.OrgId);

            Assert.NotNull(found);
            Assert.Equal(firstOrg.OrgName, found.OrgName);
        }

        [Fact]
        public void GetById_NonExisting_ShouldReturnNull()
        {
            var result = _service.GetById(BigInteger.Parse("999999"));
            Assert.Null(result);
        }

        [Fact]
        public void Update_ShouldChangeName()
        {
            _service.SeedTestData();
            var org = _service.GetAll().First();
            string newName = "Обновленное название";

            var updated = _service.Update(org.OrgId, newName);

            Assert.Equal(newName, updated.OrgName);
        }

        [Fact]
        public void Update_NonExisting_ShouldReturnNull()
        {
            var result = _service.Update(BigInteger.Parse("999999"), "Test");
            Assert.Null(result);
        }

        [Fact]
        public void Delete_ShouldRemoveOrganization()
        {
            _service.SeedTestData();
            var org = _service.GetAll().First();
            int initialCount = _service.GetAll().Count;

            bool result = _service.Delete(org.OrgId);

            Assert.True(result);
            Assert.Equal(initialCount - 1, _service.GetAll().Count);
        }

        [Fact]
        public void Delete_NonExisting_ShouldReturnFalse()
        {
            bool result = _service.Delete(BigInteger.Parse("999999"));
            Assert.False(result);
        }
    }
    // ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff
    public class ProductServiceTests
    {
        private readonly IProductService _service;
        private readonly IWarehouseService _warehouseService;
        private readonly IOrganizationService _orgService;

        public ProductServiceTests()
        {
            _service = new ProductService();
            _warehouseService = new WarehouseService();
            _orgService = new OrganizationService();
        }

        [Fact]
        public void Create_Product_ShouldHaveArticle()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);
            var warehouse = _warehouseService.GetAll().First();

            var product = new Product
            {
                Name = "Тестовый товар",
                Price = 100,
                StockQuantity = 50
            };

            var created = _service.Create(product, warehouse.WhId);

            Assert.NotNull(created);
            Assert.False(string.IsNullOrEmpty(created.Article));
            Assert.Equal(warehouse.WhId, created.WarehouseId);
        }

        [Fact]
        public void UpdateStock_ShouldChangeQuantity()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);
            var warehouse = _warehouseService.GetAll().First();

            var product = _service.Create(new Product
            {
                Name = "Товар",
                StockQuantity = 100
            }, warehouse.WhId);

            _service.UpdateStock(product.Article, 50);

            var updated = _service.GetByArticle(product.Article);
            Assert.Equal(150, updated.StockQuantity);
        }

        [Fact]
        public void UpdateStock_ShouldNotGoNegative()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);
            var warehouse = _warehouseService.GetAll().First();

            var product = _service.Create(new Product
            {
                Name = "Товар",
                StockQuantity = 10
            }, warehouse.WhId);

            _service.UpdateStock(product.Article, -20);

            var updated = _service.GetByArticle(product.Article);
            Assert.Equal(0, updated.StockQuantity);
        }
    }
}