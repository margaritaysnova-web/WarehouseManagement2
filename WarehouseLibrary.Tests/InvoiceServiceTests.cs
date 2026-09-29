using System.Linq;
using WarehouseLibrary.Services;
using Xunit;

namespace WarehouseLibrary.Tests
{
    public class InvoiceServiceTests
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;
        private readonly IOrganizationService _orgService;
        private readonly IWarehouseService _warehouseService;

        public InvoiceServiceTests()
        {
            _invoiceService = new InvoiceService();
            _productService = new ProductService();
            _orgService = new OrganizationService();
            _warehouseService = new WarehouseService();
        }

        [Fact]
        public void Create_ShouldCreateInvoice()
        {
            var invoice = _invoiceService.Create("In", 1, "Склад №1");

            Assert.NotNull(invoice);
            Assert.Equal("In", invoice.InvoiceType);
            Assert.Equal("Черновик", invoice.Status);
            Assert.NotNull(invoice.InvoiceNumber);
        }

        [Fact]
        public void AddItem_ShouldAddToInvoice()
        {
            var invoice = _invoiceService.Create("In", 1, "Склад №1");

            _invoiceService.AddItem(invoice.InvoiceId, "ART-001", "Товар", 5, 100);

            var loadedInvoice = _invoiceService.GetById(invoice.InvoiceId);
            Assert.Single(loadedInvoice.Items);
            Assert.Equal(5, loadedInvoice.Items[0].Quantity);
            Assert.Equal(500, loadedInvoice.TotalAmount);
        }

        [Fact]
        public void ProcessInvoice_ShouldChangeStatus()
        {
            var invoice = _invoiceService.Create("In", 1, "Склад №1");
            _invoiceService.AddItem(invoice.InvoiceId, "ART-001", "Товар", 5, 100);

            // Создаем товар в сервисе продуктов
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);
            var warehouse = _warehouseService.GetAll().First();

            var product = _productService.Create(
                new WarehouseData.Models.Product
                {
                    Article = "ART-001",
                    Name = "Товар",
                    StockQuantity = 10
                },
                warehouse.WhId);

            bool result = _invoiceService.ProcessInvoice(invoice.InvoiceId, _productService);

            Assert.True(result);
            Assert.Equal("Проведена", invoice.Status);

            // Проверяем, что остаток изменился
            var updatedProduct = _productService.GetByArticle("ART-001");
            Assert.Equal(15, updatedProduct.StockQuantity);
        }

        [Fact]
        public void ProcessOutcomeInvoice_ShouldDecreaseStock()
        {
            _orgService.SeedTestData();
            var orgs = _orgService.GetAll();
            _warehouseService.SeedTestData(orgs);
            var warehouse = _warehouseService.GetAll().First();

            var product = _productService.Create(
                new WarehouseData.Models.Product
                {
                    Article = "ART-002",
                    Name = "Товар для расхода",
                    StockQuantity = 100
                },
                warehouse.WhId);

            var invoice = _invoiceService.Create("Out", warehouse.WhId, warehouse.WhName);
            _invoiceService.AddItem(invoice.InvoiceId, "ART-002", "Товар для расхода", 30, 100);

            bool result = _invoiceService.ProcessInvoice(invoice.InvoiceId, _productService);

            Assert.True(result);
            Assert.Equal("Проведена", invoice.Status);

            var updatedProduct = _productService.GetByArticle("ART-002");
            Assert.Equal(70, updatedProduct.StockQuantity);
        }
    }
}