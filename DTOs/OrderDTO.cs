namespace OrderModule.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public int SenderAddressId { get; set; }
        public int ReceiverAddressId { get; set; }
        public string Description { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; }
    }
}
