namespace TestApp.Models
{
    public class OrderItemDTO
    {
        public int id { get; set; }
        public string orderId { get; set; }
        public string name { get; set; }
        public string quantity { get; set; }
        public string unit { get; set; }
    }

    public class OrderDTO
    {
        public string id { get; set; }
        public string number { get; set; }
        public string date { get; set; }
        public string providerId { get; set; }
        public string providerName { get; set; }
        public List<OrderItemDTO> orderItems { get; set; }
    }
}
