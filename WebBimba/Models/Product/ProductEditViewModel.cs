using System.ComponentModel.DataAnnotations;

namespace WebBimba.Models.Product
{
    public class ProductEditViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Назва продукту")]
        public string Name { get; set; } = String.Empty;
        [Display(Name = "Ціна продукту")]
        public decimal Price { get; set; }
        [Display(Name = "Оберіть фото на ПК")]
        public List<IFormFile>? Photos { get; set; }
    }
}
