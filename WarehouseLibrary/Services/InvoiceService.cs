using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using WarehouseData.Models;

namespace WarehouseLibrary.Services
{
    public class InvoiceService : IInvoiceService
    {
        private List<Invoice> _invoices = new List<Invoice>();

        public List<Invoice> GetAll()
        {
            return _invoices.ToList();
        }

        public List<Invoice> GetByWarehouseId(BigInteger warehouseId)
        {
            return _invoices.Where(i => i.WarehouseId == warehouseId).ToList();
        }

        public Invoice GetById(BigInteger id)
        {
            return _invoices.FirstOrDefault(i => i.InvoiceId == id);
        }

        public Invoice Create(string type, BigInteger warehouseId, string warehouseName)
        {
            var invoice = new Invoice(type, warehouseId, warehouseName);
            _invoices.Add(invoice);
            return invoice;
        }

        public void AddItem(BigInteger invoiceId, string productArticle, string productName, int quantity, decimal price)
        {
            var invoice = GetById(invoiceId);
            if (invoice != null && invoice.Status == "Черновик")
            {
                // Проверяем, есть ли уже такой товар в накладной
                var existingItem = invoice.Items.FirstOrDefault(i => i.ProductArticle == productArticle);
                if (existingItem != null)
                {
                    // Если есть - увеличиваем количество
                    existingItem.Quantity += quantity;
                }
                else
                {
                    // Если нет - добавляем новую позицию
                    invoice.Items.Add(new InvoiceItem
                    {
                        ProductArticle = productArticle,
                        ProductName = productName,
                        Quantity = quantity,
                        Price = price
                    });
                }
            }
        }

        public void RemoveItem(BigInteger invoiceId, int itemId)
        {
            var invoice = GetById(invoiceId);
            if (invoice != null && invoice.Status == "Черновик")
            {
                var item = invoice.Items.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    invoice.Items.Remove(item);
                }
            }
        }

        /// <summary>
        /// Проведение накладной - изменение остатков на складе
        /// </summary>
        public bool ProcessInvoice(BigInteger invoiceId, IProductService productService)
        {
            var invoice = GetById(invoiceId);
            if (invoice == null || invoice.Status != "Черновик")
                return false;

            // Для каждой позиции в накладной изменяем остаток
            foreach (var item in invoice.Items)
            {
                int quantityChange = item.Quantity;

                // Если это расходная накладная - делаем количество отрицательным
                if (invoice.InvoiceType == "Out")
                {
                    quantityChange = -quantityChange;
                }

                productService.UpdateStock(item.ProductArticle, quantityChange);
            }

            // Меняем статус накладной
            invoice.Status = "Проведена";
            return true;
        }

        public bool Delete(BigInteger id)
        {
            var invoice = GetById(id);
            if (invoice != null)
            {
                _invoices.Remove(invoice);
                return true;
            }
            return false;
        }
    }
}