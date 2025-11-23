namespace Ass1_C_5_OrderFastFood.Models
{
    public class Combo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<ComboItem> ComboItems { get; set; }
    }
}
