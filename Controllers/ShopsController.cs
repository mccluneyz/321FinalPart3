using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeCo.Data;
using CoffeeCo.Models;

namespace CoffeeCo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopsController : ControllerBase
{
    private readonly CoffeeDbContext _context;

    public ShopsController(CoffeeDbContext context)
    {
        _context = context;
    }

    // GET: api/shops
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Shop>>> GetShops()
    {
        try
        {
            // Query shops directly from database
            var shops = await _context.Shop
                .Where(s => !s.Deleted)
                .OrderByDescending(s => s.Rating)
                .ToListAsync();
            
            Console.WriteLine($"GET /api/shops: Found {shops.Count} shops in database");
            if (shops.Count > 0)
            {
                var firstShop = shops[0];
                Console.WriteLine($"First shop details:");
                Console.WriteLine($"  ShopID: {firstShop.ShopID}");
                Console.WriteLine($"  ShopName: {firstShop.ShopName}");
                Console.WriteLine($"  Rating: {firstShop.Rating} (type: {firstShop.Rating.GetType()})");
                Console.WriteLine($"  DateEntered: {firstShop.DateEntered} (type: {firstShop.DateEntered.GetType()})");
                Console.WriteLine($"  Favorited: {firstShop.Favorited}");
                Console.WriteLine($"  Deleted: {firstShop.Deleted}");
            }
            else
            {
                // Check if there are any shops at all (including deleted)
                var allShops = await _context.Shop.ToListAsync();
                Console.WriteLine($"Total shops in database (including deleted): {allShops.Count}");
                if (allShops.Count > 0)
                {
                    foreach (var shop in allShops.Take(5))
                    {
                        Console.WriteLine($"  Shop ID={shop.ShopID}, Name={shop.ShopName}, Deleted={shop.Deleted}, Rating={shop.Rating}, DateEntered={shop.DateEntered}");
                    }
                }
            }
            
            return Ok(shops);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
        {
            Console.WriteLine($"Database update error: {dbEx.Message}");
            Console.WriteLine($"Inner exception: {dbEx.InnerException?.Message}");
            return StatusCode(500, new { error = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching shops: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { error = $"Error: {ex.Message}. Inner: {ex.InnerException?.Message}" });
        }
    }

    // GET: api/shops/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Shop>> GetShop(int id)
    {
        var shop = await _context.Shop
            .FirstOrDefaultAsync(s => s.ShopID == id && !s.Deleted);

        if (shop == null)
        {
            return NotFound();
        }

        return shop;
    }

    // POST: api/shops
    [HttpPost]
    public async Task<ActionResult<Shop>> PostShop(Shop shop)
    {
        try
        {
            if (string.IsNullOrEmpty(shop.ShopName))
            {
                return BadRequest(new { error = "ShopName is required" });
            }

            shop.DateEntered = DateTime.Now;
            shop.Favorited = false;
            shop.Deleted = false;

            _context.Shop.Add(shop);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Shop added successfully: ID={shop.ShopID}, Name={shop.ShopName}, Rating={shop.Rating}");

            // Return the created shop
            return CreatedAtAction(nameof(GetShop), new { id = shop.ShopID }, shop);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding shop: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { error = $"Error adding shop: {ex.Message}" });
        }
    }

    // PATCH: api/shops/5/favorite
    [HttpPatch("{id}/favorite")]
    public async Task<IActionResult> ToggleFavorite(int id)
    {
        var shop = await _context.Shop
            .FirstOrDefaultAsync(s => s.ShopID == id && !s.Deleted);

        if (shop == null)
        {
            return NotFound();
        }

        shop.Favorited = !shop.Favorited;
        await _context.SaveChangesAsync();

        return Ok(new { ShopID = shop.ShopID, Favorited = shop.Favorited, message = "Favorite status updated" });
    }

    // DELETE: api/shops/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShop(int id)
    {
        try
        {
            var shop = await _context.Shop.FindAsync(id);
            if (shop == null)
            {
                Console.WriteLine($"Delete failed: Shop with ID {id} not found");
                return NotFound(new { error = $"Shop with ID {id} not found" });
            }

            Console.WriteLine($"Deleting shop: ID={shop.ShopID}, Name={shop.ShopName}");
            shop.Deleted = true;
            await _context.SaveChangesAsync();

            Console.WriteLine($"Shop {id} deleted successfully");
            return Ok(new { ShopID = id, message = "Shop deleted successfully" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting shop {id}: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
            }
            return StatusCode(500, new { error = $"Error deleting shop: {ex.Message}" });
        }
    }
}

