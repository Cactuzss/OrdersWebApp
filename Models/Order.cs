﻿namespace TestApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public DateTime? Date { get; set; }
        public int ProviderID {  get; set; }
    }
}
