namespace OrderModule.Entities
{
    public class Member
    {
        public int MemberId { get; set; } // Üye ID
        public string MemberName { get; set; } // Üye Adı
        public string NationalId { get; set; } // Üye Kimlik No
        public string Nationality { get; set; } // Üye Milleti
        public string PhoneNumber { get; set; } // Telefon Numarası
        public string Email { get; set; } // E-posta
        public string? Description { get; set; } // Açıklama

        public ICollection<Address>? Addresses { get; set; } // Üyenin adresleri
    }
}
