namespace OrderModule.DTOs
{
    public class AddressDTO
    {
        public int AddressId { get; set; }
        public int MemberId { get; set; }
        public string AddressName { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string AddressDetails { get; set; }
        public string AddressInstructions { get; set; }
        public string Description { get; set; }
    }
}
