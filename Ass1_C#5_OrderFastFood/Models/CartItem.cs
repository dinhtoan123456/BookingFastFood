namespace Ass1_C_5_OrderFastFood.Models
{
    public class CartItem
    {
        public int FoodItemId { get; set; }
        public int? ComboId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
        public bool IsCombo => ComboId.HasValue;
    }
}
