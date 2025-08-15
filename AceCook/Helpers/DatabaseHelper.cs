using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AceCook.Models;

namespace AceCook.Helpers
{
    public static class DatabaseHelper
    {
        private static readonly int MaxRetryCount = 3;
        private static readonly int RetryDelayMs = 1000;

        public static async Task<bool> TestConnectionAsync(AppDbContext context)
        {
            try
            {
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName = "Database operation")
        {
            var lastException = new Exception();
            
            for (int attempt = 1; attempt <= MaxRetryCount; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    
                    if (attempt < MaxRetryCount)
                    {
                        // Log retry attempt
                        System.Diagnostics.Debug.WriteLine($"Attempt {attempt} failed for {operationName}: {ex.Message}");
                        
                        // Wait before retry
                        await Task.Delay(RetryDelayMs * attempt);
                    }
                }
            }
            
            throw new InvalidOperationException($"Operation '{operationName}' failed after {MaxRetryCount} attempts. Last error: {lastException.Message}", lastException);
        }

        public static async Task<bool> WaitForConnectionAsync(AppDbContext context, int timeoutSeconds = 30)
        {
            var startTime = DateTime.Now;
            
            while (DateTime.Now - startTime < TimeSpan.FromSeconds(timeoutSeconds))
            {
                if (await TestConnectionAsync(context))
                {
                    return true;
                }
                
                await Task.Delay(1000); // Wait 1 second before retry
            }
            
            return false;
        }

        public static string GetConnectionStatus(AppDbContext context)
        {
            try
            {
                var canConnect = context.Database.CanConnectAsync().Result;
                return canConnect ? "Connected" : "Disconnected";
            }
            catch
            {
                return "Error";
            }
        }
    }
}
