namespace TestApp.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? Name { get; set; }
        public Decimal? Quantity { get; set; }
        public string? Unit { get; set; }
    }
}
