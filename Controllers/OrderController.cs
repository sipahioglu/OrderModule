using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderModule.Data;
using OrderModule.Entities;

namespace orderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderModuleDbContext _context;

        public OrderController(OrderModuleDbContext context)
        {
            _context = context;
        }

        // Tüm Siparişleri Getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders
                                 .Include(o => o.OrderDetails) // Sipariş detaylarını da dahil et
                                 .ToListAsync();
        }

        // Belirli Bir Siparişi Getir
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                                       .Include(o => o.OrderDetails) // Sipariş detaylarını dahil et
                                       .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return order;
        }

        // Yeni Sipariş ve Sipariş Detayları Oluştur
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            // Sipariş detayları yoksa hata döndür
            if (order.OrderDetails == null || !order.OrderDetails.Any())
                return BadRequest("Sipariş detayları boş olamaz.");

            // Stok kontrolü
            var validationResult = await ValidateOrderDetails(order.OrderDetails);
            if (validationResult != null)
                return BadRequest(validationResult);

            // Sipariş oluşturuluyor
            _context.Orders.Add(order);

            // Sipariş detayları ekleniyor ve stok güncelleniyor
            foreach (var detail in order.OrderDetails)
            {
                detail.OrderId = order.OrderId; // Sipariş Detaylarına Sipariş ID'yi ekle
                _context.OrderDetails.Add(detail);

                // Ürün stoklarını güncelle
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product != null)
                {
                    product.Quantity -= detail.ProductQuantity;
                    _context.Products.Update(product);
                }
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        // Sipariş ve İlgili Detayları Güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId)
                return BadRequest();

            // Mevcut siparişi kontrol et
            var existingOrder = await _context.Orders
                                              .Include(o => o.OrderDetails) // Detayları dahil et
                                              .FirstOrDefaultAsync(o => o.OrderId == id);

            if (existingOrder == null)
                return NotFound($"Sipariş bulunamadı (ID: {id}).");

            // Eski sipariş detayları üzerinden stok iadesi yap
            foreach (var detail in existingOrder.OrderDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product != null)
                {
                    product.Quantity += detail.ProductQuantity; // Eski sipariş detaylarını stoklara geri ekle
                    _context.Products.Update(product);
                }
            }

            // Yeni sipariş detaylarını kontrol et
            var validationResult = await ValidateOrderDetails(order.OrderDetails);
            if (validationResult != null)
                return BadRequest(validationResult);

            // Eski sipariş detaylarını kaldır
            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

            // Yeni sipariş detaylarını ekle ve stokları güncelle
            foreach (var detail in order.OrderDetails)
            {
                detail.OrderId = id;
                _context.OrderDetails.Add(detail);

                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);
                if (product != null)
                {
                    product.Quantity -= detail.ProductQuantity; // Yeni detayları stoklardan düş
                    _context.Products.Update(product);
                }
            }

            // Sipariş bilgilerini güncelle
            existingOrder.OrderName = order.OrderName;
            existingOrder.SenderAddressId = order.SenderAddressId;
            existingOrder.ReceiverAddressId = order.ReceiverAddressId;
            existingOrder.Description = order.Description;

            _context.Entry(existingOrder).State = EntityState.Modified;

            // Değişiklikleri kaydet
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(o => o.OrderId == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // Sipariş Sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                                       .Include(o => o.OrderDetails) // Sipariş Detaylarını Dahil Et
                                       .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
                return NotFound();

            // Sipariş detaylarını sil
            _context.OrderDetails.RemoveRange(order.OrderDetails);

            // Siparişi sil
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        
        private async Task<ActionResult<decimal>> CalculateTotalPrice(int orderDetailId)
        {
            // Sipariş detayını veri tabanından yükle
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Product) // Ürünü dahil et
                .ThenInclude(p => p.Category) // Kategoriyi dahil et
                .FirstOrDefaultAsync(od => od.OrderDetailId == orderDetailId);

            if (orderDetail == null)
                return NotFound($"Sipariş detayı bulunamadı (ID: {orderDetailId}).");

            // Ürün bilgisi ve kategori ID'si
            var productId = orderDetail.ProductId;
            var categoryId = orderDetail.Product.CategoryId;

            // İndirimleri bul
            var discounts = await _context.Discounts
                .Where(d =>
                    (d.ProductId == productId || d.ProductId == null) && // Ürün ID eşleşmesi veya boş olması
                    (d.CategoryId == categoryId || d.CategoryId == null) && // Kategori ID eşleşmesi veya boş olması
                    d.StartDate <= DateTime.UtcNow &&
                    d.EndDate >= DateTime.UtcNow) // Geçerli tarih aralığı
                .ToListAsync();

            // Vergileri bul
            var taxes = await _context.Taxes
                .Where(t =>
                    (t.ProductId == productId || t.ProductId == null) && // Ürün ID eşleşmesi veya boş olması
                    (t.CategoryId == categoryId || t.CategoryId == null) && // Kategori ID eşleşmesi veya boş olması
                    t.StartDate <= DateTime.UtcNow &&
                    t.EndDate >= DateTime.UtcNow) // Geçerli tarih aralığı
                .ToListAsync();

            // Başlangıç fiyatı
            decimal initialPrice = orderDetail.Price * orderDetail.ProductQuantity;

            // İndirimleri uygula
            decimal totalDiscount = 0;
            foreach (var discount in discounts)
            {
                if (discount.IsRate)
                {
                    // Oran bazlı indirim
                    totalDiscount += (initialPrice * discount.DiscountRate / 100);
                }
                else
                {
                    // Miktar bazlı indirim
                    totalDiscount += discount.DiscountAmount;
                }
            }

            // Vergileri uygula
            decimal totalTax = 0;
            foreach (var tax in taxes)
            {
                if (tax.IsRate)
                {
                    // Oran bazlı vergi
                    totalTax += ((initialPrice - totalDiscount) * tax.TaxRate / 100);
                }
                else
                {
                    // Miktar bazlı vergi
                    totalTax += tax.TaxAmount;
                }
            }

            // Son fiyat hesaplama
            decimal finalPrice = (initialPrice - totalDiscount) + totalTax;

            // Sonucu döndür
            return Ok(finalPrice);
        }

        private async Task<string?> ValidateOrderDetails(IEnumerable<OrderDetail> orderDetails)
        {
            foreach (var detail in orderDetails)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == detail.ProductId);

                if (product == null)
                    return $"Ürün bulunamadı (ID: {detail.ProductId}).";

                if (product.Quantity < detail.ProductQuantity)
                    return $"Ürün stokta yeterli değil (ID: {detail.ProductId}, Mevcut: {product.Quantity}, İstenen: {detail.ProductQuantity}).";
            }

            return null; // Her şey yolunda
        }
    }
}
