namespace Ass1_C_5_OrderFastFood.Models
{
    public class FoodCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<FoodItem> FoodItems { get; set; }
    }
}
