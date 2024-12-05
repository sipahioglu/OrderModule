namespace OrderModule.DTOs
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }
        public string InvoiceName { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Description { get; set; }
    }
}
