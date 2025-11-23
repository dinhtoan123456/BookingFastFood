namespace Ass1_C_5_OrderFastFood.Models
{
    public enum OrderStatus { Pending, Processing, Delivering, Delivered, Cancelled }
    public class Order
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Total { get; set; }
        public string PaymentMethod { get; set; } // e.g., "COD", "StripeTest"
        public string DeliveryAddress { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
