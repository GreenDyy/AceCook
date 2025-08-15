using System;
using System.Threading.Tasks;
using AceCook.Models;
using AceCook.Repositories;

namespace AceCook.Tests
{
    public static class TestInventoryOperations
    {
        public static async Task TestInventoryUpdate(AppDbContext context)
        {
            try
            {
                var repository = new InventoryRepository(context);
                
                Console.WriteLine("=== Testing Inventory Operations ===");
                
                // Test 1: Get inventory data
                Console.WriteLine("1. Testing GetInventoryByIdAsync...");
                var inventory = await repository.GetInventoryByIdAsync("SP001", "KHO001");
                if (inventory != null)
                {
                    Console.WriteLine($"   Found inventory: SP001 in KHO001, Current stock: {inventory.SoLuongTonKho}");
                }
                else
                {
                    Console.WriteLine("   No inventory found for SP001 in KHO001");
                }
                
                // Test 2: Test update operation
                Console.WriteLine("2. Testing UpdateInventoryAsync...");
                if (inventory != null)
                {
                    var originalStock = inventory.SoLuongTonKho ?? 0;
                    var newStock = originalStock - 5; // Giả sử xuất 5 sản phẩm
                    
                    if (newStock >= 0)
                    {
                        inventory.SoLuongTonKho = newStock;
                        var success = await repository.UpdateInventoryAsync(inventory);
                        Console.WriteLine($"   Update result: {success}");
                        
                        if (success)
                        {
                            // Verify the update
                            var updatedInventory = await repository.GetInventoryByIdAsync("SP001", "KHO001");
                            Console.WriteLine($"   Stock after update: {updatedInventory?.SoLuongTonKho}");
                            
                            // Restore original stock
                            inventory.SoLuongTonKho = originalStock;
                            await repository.UpdateInventoryAsync(inventory);
                            Console.WriteLine($"   Stock restored to: {originalStock}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"   Cannot reduce stock to negative: {newStock}");
                    }
                }
                
                Console.WriteLine("=== Test completed ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed with error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        
        public static async Task TestDatabaseConnection(AppDbContext context)
        {
            try
            {
                Console.WriteLine("=== Testing Database Connection ===");
                
                var canConnect = await context.Database.CanConnectAsync();
                Console.WriteLine($"Database connection: {(canConnect ? "Success" : "Failed")}");
                
                if (canConnect)
                {
                    var repository = new InventoryRepository(context);
                    var totalItems = await repository.GetTotalItemsAsync();
                    Console.WriteLine($"Total inventory items: {totalItems}");
                }
                
                Console.WriteLine("=== Connection test completed ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection test failed: {ex.Message}");
            }
        }
    }
}
