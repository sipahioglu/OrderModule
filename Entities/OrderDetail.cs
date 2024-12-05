namespace OrderModule.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; } // Sipariş Detay ID
        public int OrderId { get; set; } // Sipariş ID
        public int LineNumber { get; set; } // Kalem Numarası
        public int ProductId { get; set; } // Ürün ID
        public int ProductQuantity { get; set; } // Ürün Adedi
        public decimal Price { get; set; } // Fiyat

        public Order? Order { get; set; } // Sipariş ile ilişki
        public Product? Product { get; set; } // Ürün ile ilişki
    }
}
