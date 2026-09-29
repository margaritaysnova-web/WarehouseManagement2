using System.Linq;
using WarehouseData.Models;
using WarehouseLibrary.Services;
using Xunit;

namespace WarehouseLibrary.Tests
{
    public class WarehouseServiceTests
    {
        private readonly IWarehouseService _warehouseService;
        private readonly IOrganizationService _orgService;

        public WarehouseServiceTests()
        {
            _warehouseService = new WarehouseService();
            _orgService = new OrganizationService();
        }

        [Fact]
        public void SeedTestData_ShouldCreateWarehouses()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();

            _warehouseService.SeedTestData(orgs);

            var warehouses = _warehouseService.GetAll();
            Assert.NotEmpty(warehouses);
            Assert.Equal(3, warehouses.Count);
        }

        [Fact]
        public void GetByOrganizationId_ShouldReturnOnlyOrgWarehouses()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);

            var firstOrg = orgs.First();
            var warehouses = _warehouseService.GetByOrganizationId(firstOrg.OrgId);

            Assert.All(warehouses, w => Assert.Equal(firstOrg.OrgId, w.OrgId));
        }

        [Fact]
        public void Create_ShouldAddWarehouse()
        {
            _orgService.SeedTestData();
            var org = _orgService.GetAll().First();

            var warehouse = _warehouseService.Create("Тестовый склад", "Тестовый адрес", org.OrgId);

            Assert.NotNull(warehouse);
            Assert.Equal("Тестовый склад", warehouse.WhName);
            Assert.Equal(org.OrgId, warehouse.OrgId);
        }

        [Fact]
        public void Update_ShouldChangeWarehouse()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);
            var warehouse = _warehouseService.GetAll().First();

            var updated = _warehouseService.Update(warehouse.WhId, "Новое название", "Новый адрес");

            Assert.Equal("Новое название", updated.WhName);
            Assert.Equal("Новый адрес", updated.WhAddress);
        }

        [Fact]
        public void Delete_ShouldRemoveWarehouse()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);
            var warehouse = _warehouseService.GetAll().First();

            bool result = _warehouseService.Delete(warehouse.WhId);

            Assert.True(result);
            Assert.Null(_warehouseService.GetById(warehouse.WhId));
        }
    }
}