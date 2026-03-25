using ReceiptProject1.DTOs.ItemDTOs;

namespace ReceiptProject1.DTOs.ReceiptDTOs
{
    public class CreateReceiptDTO
    {
        public DateTime Date { get; set; }
        public List<CreateItemDTO> Items { get; set; }
        public string StoreName { get; set; }
    }
}