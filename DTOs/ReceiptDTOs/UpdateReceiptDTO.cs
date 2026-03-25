using ReceiptProject1.DTOs.ItemDTOs;

namespace ReceiptProject1.DTOs.ReceiptDTOs
{
    public class UpdateReceiptDTO
    {
        public string StoreName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public List<UpdateItemDTO> Items { get; set; } = new();
    }
}
