namespace OrderModule.Entities
{
    public class PaymentDetail
    {
        public int PaymentDetailId { get; set; } // Ödeme Detay ID
        public int OrderDetailId { get; set; } // Sipariş Detay ID
        public int ProductId { get; set; } // Ürün ID
        public int? DiscountId { get; set; } // İndirim ID
        public int? TaxId { get; set; } // Vergi ID
        public decimal ProductPrice { get; set; } // Ürün Fiyatı
        public bool IsRateBased { get; set; } // Orana Dayalı mı?
        public decimal ProductDiscountRate { get; set; } // Ürün İndirim Oranı
        public decimal ProductDiscountAmount { get; set; } // Ürün İndirim Miktarı

        public OrderDetail? OrderDetail { get; set; } // Sipariş Detayı ile ilişki
        public Product? Product { get; set; } // Ürün ile ilişki
        public Discount? Discount { get; set; } // İndirim ile ilişki
        public Tax? Tax { get; set; } // Vergi ile ilişki
    }
}
