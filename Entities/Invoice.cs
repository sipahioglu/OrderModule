namespace OrderModule.Entities
{
    public class Invoice
    {
        public int InvoiceId { get; set; } // Fatura ID
        public string InvoiceName { get; set; } // Fatura Adı
        public int OrderId { get; set; } // Sipariş ID
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; } // Açıklama

        public Order? Order { get; set; } // Sipariş ile ilişki
    }
}
