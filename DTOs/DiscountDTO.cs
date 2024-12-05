namespace OrderModule.DTOs
{
    public class DiscountDTO
    {
        public int DiscountId { get; set; } // İndirim ID
        public int? CategoryId { get; set; } // Kategori ID
        public int? ProductId { get; set; } // Ürün ID
        public string DiscountName { get; set; } // İndirim Adı
        public bool IsRate { get; set; } //oran mı
        public decimal DiscountRate { get; set; } // İndirim Oranı
        public decimal DiscountAmount { get; set; } //İndirim miktarı
        public DateTime StartDate { get; set; } // Başlangıç Tarihi
        public DateTime EndDate { get; set; } // Bitiş Tarihi
        public string? Description { get; set; } // Açıklama
    }
}
