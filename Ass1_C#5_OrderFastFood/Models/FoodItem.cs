using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ass1_C_5_OrderFastFood.Models
{
    public class FoodItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên món ăn không được để trống.")]
        [DisplayName("Tên Món ăn")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá không được để trống.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
        [DisplayName("Giá")]
        public decimal Price { get; set; }

        [DisplayName("Mô tả")]
        public string Description { get; set; }

        [DisplayName("Đường dẫn ảnh")]
        public string? ImageUrl { get; set; }

        // QUAN TRỌNG: Trường này cần được chọn trên form
        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [DisplayName("Danh mục")]
        public int CategoryId { get; set; }
        public FoodCategory? Category { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng món ăn.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0.")]
        [DisplayName("Số lượng")]
        public int Quantity { get; set; }

        [DisplayName("Đang bán")]
        public bool IsActive { get; set; }
    }
}
