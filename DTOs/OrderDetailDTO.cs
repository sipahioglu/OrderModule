namespace OrderModule.DTOs
{
    public class OrderDetailDTO
    {
        public int OrderDetailId { get; set; }
        public int LineNumber { get; set; }
        public int ProductId { get; set; }
        public int ProductQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal LastPrice { get; set; }
        public List<DiscountDTO> Discounts { get; set; }
        public List<TaxDTO> Taxs { get; set; }
    }
}
