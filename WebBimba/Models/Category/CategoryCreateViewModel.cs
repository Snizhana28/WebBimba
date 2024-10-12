using System.ComponentModel.DataAnnotations;

namespace WebBimba.Models.Category;

public class CategoryCreateViewModel // модель для створення категорії
{
    [Display(Name = "Назва категорії")] //відображення назви поля на сторінці
    public string Name { get; set; } = String.Empty; //властивість для зберігання назви категорії
    //Тип для передачі файлі на сервер - із сторінки хочу отримати файл із <input type="file"/>
    [Display(Name = "Оберіть фото на ПК")] //відображення назви поля на сторінці
    public IFormFile? Photo { get; set; } //властивість для зберігання файлу
    [Display(Name = "Короткий опис")] //відображення назви поля на сторінці
    public string Description { get; set; } = string.Empty; //властивість для зберігання короткого опису
}
