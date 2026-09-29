using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace WarehouseData.Models
{
    /// <summary>
    /// Строка накладной - одна позиция товара в накладной
    /// </summary>
    [Table("invoice_items")]
    public class InvoiceItem
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Артикул товара
        /// </summary>
        [Column("product_article")]
        public string ProductArticle { get; set; }

        /// <summary>
        /// Название товара (для отображения)
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Количество товара в накладной
        /// </summary>
        [Column("quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Цена за единицу на момент создания накладной
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// Общая стоимость позиции (цена * количество)
        /// </summary>
        public decimal TotalPrice => Price * Quantity;

        public InvoiceItem() { }
    }
}