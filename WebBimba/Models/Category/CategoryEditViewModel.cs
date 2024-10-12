using System.ComponentModel.DataAnnotations;
namespace WebBimba.Models.Category;

public class CategoryEditViewModel //модель для редагування категорії
{
    public int Id { get; set; } // Ідентифікатор категорії

    [Required(ErrorMessage = "Поле 'Назва' є обов'язковим")] // Правило валідації
    public string Name { get; set; } // Назва категорії
    public string Description { get; set; } // Опис категорії
    public IFormFile? Photo { get; set; } // нове зображення
    public string? ExistingImage { get; set; } // існуюче зображення
}
