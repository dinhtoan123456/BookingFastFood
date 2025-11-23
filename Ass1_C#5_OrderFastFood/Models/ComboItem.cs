namespace Ass1_C_5_OrderFastFood.Models
{
    public class ComboItem
    {
        public int Id { get; set; }
        public int ComboId { get; set; }
        public Combo Combo { get; set; }
        public int FoodItemId { get; set; }
        public FoodItem FoodItem { get; set; }
        public int Quantity { get; set; }
    }
}
