using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderModule.Data;
using OrderModule.Entities;

namespace orderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly OrderModuleDbContext _context;

        public InvoiceController(OrderModuleDbContext context)
        {
            _context = context;
        }

        // Tüm Faturaları Getir
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invoice>>> GetInvoices()
        {
            return await _context.Invoices
                                 .Include(i => i.Order) // Sipariş bilgilerini dahil et
                                 .ToListAsync();
        }

        // Belirli Bir Faturayı Getir
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices
                                        .Include(i => i.Order) // Sipariş bilgilerini dahil et
                                        .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
                return NotFound();

            return invoice;
        }

        // Belirli Bir Sipariş için Fatura Oluştur
        [HttpPost("{orderId}")]
        public async Task<ActionResult<Invoice>> CreateInvoice(int orderId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Siparişi Bul
                var order = await _context.Orders
                                           .Include(o => o.OrderDetails) // Sipariş detaylarını dahil et
                                           .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                    return NotFound($"Sipariş bulunamadı (ID: {orderId})");

                // Toplam Tutar Hesapla
                var totalAmount = order.OrderDetails.Sum(d => d.Price * d.ProductQuantity);

                // Fatura Oluştur
                var invoice = new Invoice
                {
                    InvoiceName = $"Fatura - Sipariş {order.OrderId}",
                    OrderId = order.OrderId,
                    TotalAmount = totalAmount,
                    InvoiceDate = DateTime.UtcNow,
                    Description = $"Sipariş #{order.OrderId} için oluşturulan fatura"
                };

                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();

                // İşlemi başarılı bir şekilde tamamla
                await transaction.CommitAsync();

                // Yeni fatura bilgisiyle cevap dön
                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, invoice);
            }
            catch (Exception ex)
            {
                // Hata durumunda rollback
                await transaction.RollbackAsync();

                // Hata mesajını loglamak veya istemciye dönmek için kullanılabilir
                return StatusCode(500, $"Fatura oluşturulurken bir hata oluştu: {ex.Message}");
            }
            
        }

        // Faturayı Güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceId)
                return BadRequest("Fatura ID'leri eşleşmiyor.");

            // Transaction başlat
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Faturayı kontrol et
                var existingInvoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceId == id);
                if (existingInvoice == null)
                    return NotFound($"Fatura bulunamadı (ID: {id})");

                // Mevcut faturayı güncelle
                existingInvoice.InvoiceName = invoice.InvoiceName;
                existingInvoice.TotalAmount = invoice.TotalAmount;
                existingInvoice.InvoiceDate = invoice.InvoiceDate;
                existingInvoice.Description = invoice.Description;

                _context.Entry(existingInvoice).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                // İşlemi başarılı bir şekilde tamamla
                await transaction.CommitAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Rollback işlemi
                await transaction.RollbackAsync();

                // Faturanın mevcut olmadığını kontrol et
                if (!_context.Invoices.Any(i => i.InvoiceId == id))
                    return NotFound($"Fatura bulunamadı (ID: {id})");

                // Hata durumu
                return StatusCode(500, "Güncelleme sırasında bir eşzamanlılık hatası oluştu.");
            }
            catch (Exception ex)
            {
                // Rollback işlemi
                await transaction.RollbackAsync();

                // Genel hata mesajı
                return StatusCode(500, $"Fatura güncellenirken bir hata oluştu: {ex.Message}");
            }
        }

        // Faturayı Sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
                return NotFound();

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
