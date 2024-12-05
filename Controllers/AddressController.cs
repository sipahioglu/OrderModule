using Microsoft.AspNetCore.Mvc;
using OrderModule.Data;
using OrderModule.Entities;
using Microsoft.EntityFrameworkCore;

namespace orderManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly OrderModuleDbContext _context;

        public AddressController(OrderModuleDbContext context)
        {
            _context = context;
        }

        // Üye ID'ye göre adresleri getir
        [HttpGet("GetAddressesByUserId/{userId}")]
        public async Task<ActionResult<List<Address>>> GetAddressesByUserId(int userId)
        {
            // Veritabanında ilgili adresleri bul
            var addresses = await _context.Addresses
                .Where(a => a.MemberId == userId)
                .ToListAsync();

            if (addresses == null || !addresses.Any())
            {
                return NotFound($"User with ID {userId} has no addresses.");
            }

            return Ok(addresses);
        }
    }
}
