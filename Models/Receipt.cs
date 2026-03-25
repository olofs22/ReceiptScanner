namespace ReceiptProject1.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string StoreName { get; set; }
        public DateTime Date { get; set; }
        public ICollection<Item> Items { get; set; }
 
    }
}
