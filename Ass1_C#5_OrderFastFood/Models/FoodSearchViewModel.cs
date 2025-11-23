namespace Ass1_C_5_OrderFastFood.Models
{
    public class FoodSearchViewModel
    {
        public string? Name { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public string? Category { get; set; }
        public int? CategoryId { get; set; }
        public string? Description { get; set; }

        public List<FoodItem>? Results { get; set; }
    }
}
