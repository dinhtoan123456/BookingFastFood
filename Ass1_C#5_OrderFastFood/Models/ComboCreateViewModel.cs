using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Ass1_C_5_OrderFastFood.Models
{
    public class ComboCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Combo phải có ít nhất một món ăn.")]
        public int[] SelectedFoodItemIds { get; set; }

        [Required]
        public int[] Quantities { get; set; }
        // Thêm ảnh cho combo
        [Display(Name = "Ảnh Combo")]
        public IFormFile? ImageFile { get; set; }
    }
}
