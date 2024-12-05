using System.Net;

namespace OrderModule.Entities
{
    public class Order
    {
        public int OrderId { get; set; } // Sipariş ID
        public string OrderName { get; set; } // Sipariş Adı
        public int SenderAddressId { get; set; } // Gönderici Adres ID
        public int ReceiverAddressId { get; set; } // Alıcı Adres ID
        public string? Description { get; set; } // Açıklama

        public Address? SenderAddress { get; set; } // Gönderici Adresi
        public Address? ReceiverAddress { get; set; } // Alıcı Adresi
        public ICollection<OrderDetail>? OrderDetails { get; set; } // Siparişin detayları
    }
}
