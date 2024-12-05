namespace OrderModule.Entities
{
    public class Tax
    {
        public int TaxId { get; set; } // Vergi ID
        public int? CategoryId { get; set; } // Kategori ID
        public int? ProductId { get; set; } // Ürün ID
        public string TaxName { get; set; } // Vergi Adı
        public bool IsRate { get; set; } //oran mı
        public decimal TaxRate { get; set; } // Vergi Oranı
        public decimal TaxAmount { get; set; } //Vergi miktarı
        public DateTime StartDate { get; set; } // Başlangıç Tarihi
        public DateTime EndDate { get; set; } // Bitiş Tarihi
        public string? Description { get; set; } // Açıklama

        public Category? Category { get; set; } // Kategori ile ilişki
        public Product? Product { get; set; } // Ürün ile ilişki
    }
}
