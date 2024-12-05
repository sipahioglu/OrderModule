namespace OrderModule.Entities
{
    public class Product
    {
        public int ProductId { get; set; } // Ürün ID
        public int CategoryId { get; set; } // Kategori ID
        public string ProductName { get; set; } // Ürün Adı
        public int Quantity { get; set; } // Ürün Adedi
        public decimal Price { get; set; } // Ürün Fiyatı
        public string? Description { get; set; } // Açıklama

        public Category? Category { get; set; } // Kategori ile ilişki
        public ICollection<Tax>? Taxes { get; set; } // Ürüne bağlı vergiler
    }
}
