using Ass1_C_5_OrderFastFood.Data;
using Ass1_C_5_OrderFastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace Ass1_C_5_OrderFastFood
{
    public static class MinimalApis
    {
        public static void MapMinimalApis(this WebApplication app)
        {
            // ============================
            //          CLIENT API
            // ============================
            var client = app.MapGroup("/api/client");

            // --- FOOD LIST ---
            client.MapGet("/food", async (ApplicationDbContext db) =>
            {
                return await db.FoodItems
                    .Include(f => f.Category)
                    .ToListAsync();
            });

            // --- FOOD DETAIL ---
            client.MapGet("/food/{id:int}", async (int id, ApplicationDbContext db) =>
            {
                var item = await db.FoodItems
                    .Include(f => f.Category)
                    .FirstOrDefaultAsync(f => f.Id == id);

                return item is null ? Results.NotFound() : Results.Ok(item);
            });

            // --- COMBO LIST ---
            client.MapGet("/combos", async (ApplicationDbContext db) =>
            {
                return await db.Combos
                    .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.FoodItem)
                    .ToListAsync();
            });

            // --- COMBO DETAIL ---
            client.MapGet("/combos/{id:int}", async (int id, ApplicationDbContext db) =>
            {
                var combo = await db.Combos
                    .Include(c => c.ComboItems)
                    .ThenInclude(ci => ci.FoodItem)
                    .FirstOrDefaultAsync(c => c.Id == id);

                return combo is null ? Results.NotFound() : Results.Ok(combo);
            });

            // --- ORDER LIST FOR USER ---
            client.MapGet("/orders/user/{userId}", async (string userId, ApplicationDbContext db) =>
            {
                return await db.Orders
                    .Where(o => o.ApplicationUserId == userId)
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                    .ToListAsync();
            });

            // --- CREATE ORDER ---
            client.MapPost("/orders", async (Order order, ApplicationDbContext db) =>
            {
                order.CreatedAt = DateTime.Now;
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return Results.Created($"/api/client/orders/{order.Id}", order);
            });

            // ============================
            //          ADMIN API
            // ============================
            var admin = app.MapGroup("/api/admin");

            // ===== FOOD =====
            admin.MapGet("/food", async (ApplicationDbContext db) =>
            {
                return await db.FoodItems.ToListAsync();
            });

            admin.MapPost("/food", async (FoodItem item, ApplicationDbContext db) =>
            {
                db.FoodItems.Add(item);
                await db.SaveChangesAsync();
                return Results.Created($"/api/admin/food/{item.Id}", item);
            });

            admin.MapPut("/food/{id:int}", async (int id, FoodItem update, ApplicationDbContext db) =>
            {
                var item = await db.FoodItems.FindAsync(id);
                if (item is null) return Results.NotFound();

                item.Name = update.Name;
                item.Description = update.Description;
                item.Price = update.Price;
                item.ImageUrl = update.ImageUrl;
                item.CategoryId = update.CategoryId;
                item.Quantity = update.Quantity;
                item.IsActive = update.IsActive;

                await db.SaveChangesAsync();
                return Results.Ok(item);
            });

            admin.MapDelete("/food/{id:int}", async (int id, ApplicationDbContext db) =>
            {
                var item = await db.FoodItems.FindAsync(id);
                if (item is null) return Results.NotFound();

                db.FoodItems.Remove(item);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // ===== COMBO =====
            admin.MapGet("/combos", async (ApplicationDbContext db) =>
            {
                return await db.Combos
                    .Include(c => c.ComboItems)
                    .ToListAsync();
            });

            admin.MapPost("/combos", async (Combo combo, ApplicationDbContext db) =>
            {
                db.Combos.Add(combo);
                await db.SaveChangesAsync();
                return Results.Created($"/api/admin/combos/{combo.Id}", combo);
            });

            admin.MapPut("/combos/{id:int}", async (int id, Combo update, ApplicationDbContext db) =>
            {
                var combo = await db.Combos.FindAsync(id);
                if (combo is null) return Results.NotFound();

                combo.Name = update.Name;
                combo.Description = update.Description;
                combo.Price = update.Price;

                await db.SaveChangesAsync();
                return Results.Ok(combo);
            });

            admin.MapDelete("/combos/{id:int}", async (int id, ApplicationDbContext db) =>
            {
                var combo = await db.Combos.FindAsync(id);
                if (combo is null) return Results.NotFound();

                db.Combos.Remove(combo);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // ===== ORDER MANAGEMENT =====
            admin.MapGet("/orders", async (ApplicationDbContext db) =>
            {
                return await db.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                    .ToListAsync();
            });

            admin.MapPut("/orders/{id:int}/status", async (int id, Order update, ApplicationDbContext db) =>
            {
                var order = await db.Orders.FindAsync(id);
                if (order is null) return Results.NotFound();

                order.Status = update.Status;
                await db.SaveChangesAsync();
                return Results.Ok(order);
            });

            // ===== USERS =====
            admin.MapGet("/users", async (ApplicationDbContext db) =>
            {
                return await db.Users.ToListAsync();
            });

            admin.MapGet("/users/{id}", async (string id, ApplicationDbContext db) =>
            {
                var user = await db.Users.FindAsync(id);
                return user is null ? Results.NotFound() : Results.Ok(user);
            });

            admin.MapDelete("/users/{id}", async (string id, ApplicationDbContext db) =>
            {
                var user = await db.Users.FindAsync(id);
                if (user is null) return Results.NotFound();

                db.Users.Remove(user);
                await db.SaveChangesAsync();
                return Results.Ok();
            });
        }
    }

}
