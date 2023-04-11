namespace BBQLover.webapp.Models
{
    public class Delivery
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? More { get; set; }
        public List<string> Orders { get; set; }
        public string? OrderNum { get; set; }
        public string? Payment { get; set; }
        public string? Tel { get; set; }
        public int Total { get; set; }
    }
}
