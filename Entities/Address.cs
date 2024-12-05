namespace OrderModule.Entities
{
    public class Address
    {
        public int AddressId { get; set; } // Adres ID
        public int MemberId { get; set; } // Üye ID
        public string AddressName { get; set; } // Adres Adı
        public string City { get; set; } // İl
        public string District { get; set; } // İlçe
        public string AddressDetail { get; set; } // Adres Detayı
        public string? AddressDescription { get; set; } // Açıklama

        public Member? Member { get; set; } // Üye ile ilişki
    }
}
