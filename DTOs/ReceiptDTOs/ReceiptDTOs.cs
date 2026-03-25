using ReceiptProject1.DTOs.ItemDTOs;

namespace ReceiptProject1.DTOs.ReceiptDTOs
{
    public class ReceiptDTO
    {
        public int Id { get; set; }
        public string StoreName { get; set; }
        public DateTime Date { get; set; }
        public List<ItemDTO> Items { get; set; }
    }
}
