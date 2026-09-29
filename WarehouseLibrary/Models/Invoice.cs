using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace WarehouseData.Models
{
    /// <summary>
    /// Накладная - документ на перемещение товаров (приход или расход)
    /// </summary>
    [Table("invoices")]
    public class Invoice
    {
        public static BigInteger InvoiceCounter = BigInteger.Zero;

        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public BigInteger InvoiceId { get; set; }

        /// <summary>
        /// Номер накладной (для отображения пользователю)
        /// </summary>
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Дата создания накладной
        /// </summary>
        [Column("created_date")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Тип накладной: "Приход" или "Расход"
        /// </summary>
        [Column("invoice_type")]
        public string InvoiceType { get; set; } // "In" - приход, "Out" - расход

        /// <summary>
        /// ID склада, на который/с которого перемещаются товары
        /// </summary>
        [Column("warehouse_id")]
        public BigInteger WarehouseId { get; set; }

        /// <summary>
        /// Название склада (для отображения)
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// Статус накладной: "Черновик", "Проведена"
        /// </summary>
        [Column("status")]
        public string Status { get; set; } = "Черновик";

        /// <summary>
        /// Список позиций в накладной
        /// </summary>
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        /// <summary>
        /// Общая сумма по накладной
        /// </summary>
        public decimal TotalAmount
        {
            get
            {
                decimal total = 0;
                foreach (var item in Items)
                {
                    total += item.TotalPrice;
                }
                return total;
            }
        }

        public Invoice()
        {
            InvoiceId = ++InvoiceCounter;
            InvoiceNumber = $"НК-{DateTime.Now:yyyyMMdd}-{InvoiceId}";
        }

        public Invoice(string type, BigInteger warehouseId, string warehouseName)
        {
            InvoiceId = ++InvoiceCounter;
            InvoiceNumber = $"НК-{DateTime.Now:yyyyMMdd}-{InvoiceId}";
            InvoiceType = type;
            WarehouseId = warehouseId;
            WarehouseName = warehouseName;
            CreatedDate = DateTime.Now;
            Status = "Черновик";
        }
    }
}