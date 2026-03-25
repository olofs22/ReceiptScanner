namespace ReceiptProject1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string EmailAdress { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Receipt> Receipts { get; set; }
    }
}
