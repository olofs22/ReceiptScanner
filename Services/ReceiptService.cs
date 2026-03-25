using Microsoft.EntityFrameworkCore;
using ReceiptProject1.Data;
using ReceiptProject1.DTOs;
using ReceiptProject1.DTOs.ItemDTOs;
using ReceiptProject1.DTOs.ReceiptDTOs;
using ReceiptProject1.Models;

namespace ReceiptProject1.Services
{
    public class ReceiptService
    {
        private readonly AppDbContext _db;

        public ReceiptService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<ReceiptDTO>> GetAllAsync(int userId)
        {
            return await _db.Receipts
                .Where(r => r.UserId == userId)
                .Include(r => r.Items)
                .Select(r => new ReceiptDTO
                {
                    Id = r.Id,
                    StoreName = r.StoreName,
                    Date = r.Date,
                    Items = r.Items.Select(i => new ItemDTO
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Price = i.Price
                    }).ToList()
                })
                .ToListAsync();
        }
        public async Task<ReceiptDTO?> GetByIdAsync(int id, int userId)
        {
            return await _db.Receipts
                .Include(r => r.Items)
                .Where(r => r.Id == id && r.UserId == userId)
                .Select(r => new ReceiptDTO
                {
                    Id = r.Id,
                    StoreName = r.StoreName,
                    Date = r.Date,
                    Items = r.Items.Select(i => new ItemDTO
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Price = i.Price
                    }).ToList()
                })
            .FirstOrDefaultAsync();
        }
        public async Task<Receipt> CreateAsync(CreateReceiptDTO crdto, int userId)
        {
            var receipt = new Receipt
            {
                StoreName = crdto.StoreName,
                Date = crdto.Date,
                UserId = userId,
                Items = crdto.Items.Select(i => new Item
                {
                    Title = i.Title,
                    Price = i.Price
                }).ToList()
            };
            _db.Receipts.Add(receipt);
            await _db.SaveChangesAsync();
            return receipt;
        }
        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var receipts = await _db.Receipts
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (receipts == null) return false;

            _db.Receipts.Remove(receipts);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
