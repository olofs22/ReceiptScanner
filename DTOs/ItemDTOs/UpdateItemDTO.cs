namespace ReceiptProject1.DTOs.ItemDTOs
{
    public class UpdateItemDTO
    {
        public int? Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
