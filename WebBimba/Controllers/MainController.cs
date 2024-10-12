using Microsoft.AspNetCore.Mvc;
using WebBimba.Data;
using WebBimba.Data.Entities;
using WebBimba.Interfaces;
using WebBimba.Models.Category;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebBimba.Controllers
{
    // Головний контролер для взаємодії з категоріями
    public class MainController : Controller
    {
        // Контекст бази даних для взаємодії з таблицями
        private readonly AppBimbaDbContext _dbContext;
        // Інтерфейс для роботи з зображеннями
        private readonly IImageWorker _imageWorker;
        //Зберігає різну інформацію про MVC проект
        private readonly IWebHostEnvironment _environment;
        //DI - Depencecy Injection
        // Конструктор з впровадженням залежностей (DI)
        public MainController(AppBimbaDbContext context,
            IWebHostEnvironment environment, IImageWorker imageWorker)
        {
            _dbContext = context;  // Ініціалізуємо контекст бази даних
            _environment = environment; // Ініціалізуємо середовище
            _imageWorker = imageWorker; // Ініціалізуємо роботу з зображеннями
        }

        // Метод (action) для відображення списку категорій
        public IActionResult Index()
        {
            // Отримуємо всі категорії з бази даних і передаємо в модель для відображення у View
            var model = _dbContext.Categories.ToList();
            return View(model);
        }

        [HttpGet] //це означає, що буде відображатися сторінки для перегляду
        // Метод, що повертає форму для створення нової категорії
        public IActionResult Create()
        {
            //Ми повертає View - пусту, яка відобраєате сторінку де потрібно ввести дані для категорії
            return View();
        }

        [HttpPost] //це означає, що ми отримуємо дані із форми від клієнта
        public IActionResult Create(CategoryCreateViewModel model)
        {
            var entity = new CategoryEntity(); //створюємо новий об'єкт категорії
            //Збережння в Базу даних інформації
            var dirName = "uploading"; //створюємо папку для збереження файлів
            var dirSave = Path.Combine(_environment.WebRootPath, dirName); //повний шлях до папки
            // перевірка чи існує папка
            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }
            //перевірка чи файл був вибраний
            if (model.Photo != null)
            {
                entity.Image = _imageWorker.Save(model.Photo); //зберігаємо файл
            }
            //заповнюємо об'єкт категорії даними з форми
            entity.Name = model.Name; 
            entity.Description = model.Description;
            //додаємо об'єкт в контекст бази даних
            _dbContext.Categories.Add(entity);
            //зберігаємо зміни в базі даних
            _dbContext.SaveChanges();
            //Переходимо до списку усіх категорій, тобото визиваємо метод Index нашого контролера
            return Redirect("/");
        }

        [HttpPost]
        // Метод для видалення категорії
        public IActionResult Delete(int id)
        {
            var category = _dbContext.Categories.Find(id); // Знаходимо категорію по id
            if (category == null) // Якщо категорію не знайдено
            {
                return NotFound(); // Повертаємо помилку 404
            }

            if (!string.IsNullOrEmpty(category.Image)) // Якщо у категорії є зображення
            {
                _imageWorker.Delete(category.Image); // Видаляємо зображення
            }
            _dbContext.Categories.Remove(category); // Видаляємо категорію
            _dbContext.SaveChanges(); // Зберігаємо зміни в базі даних

            return Json(new { text="Ми його видалили" }); // Вертаю об'єкт у відповідь
        }
        [HttpGet]
        public IActionResult Edit(int id) // Метод для редагування категорії
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.Id == id); // Знаходимо категорію по id
            if (category == null) // Якщо категорію не знайдено
            {
                return NotFound(); // Повертаємо помилку 404
            }
             
            var model = new CategoryEditViewModel // Створюємо модель для відображення
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ExistingImage = category.Image
            };
            return View(model); // Переходимо на сторінку редагування
        }
        [HttpPost]
        // Метод для редагування категорії
        public IActionResult Edit(CategoryEditViewModel model) // Метод для редагування категорії
        {
            var entity = _dbContext.Categories.FirstOrDefault(c => c.Id == model.Id); // Знаходимо категорію по id
            if (entity == null) // Якщо категорію не знайдено
            {
                return NotFound(); // Повертаємо помилку 404
            }

            entity.Name = model.Name; // Змінюємо назву категорії
            entity.Description = model.Description; // Змінюємо опис категорії

            if (model.Photo != null) // Якщо було вибрано нове зображення
            {
                var dirName = "uploading"; // Папка для збереження файлів
                var dirSave = Path.Combine(_environment.WebRootPath, dirName); // Повний шлях до папки
                if (!Directory.Exists(dirSave)) // Перевірка чи існує папка
                {
                    Directory.CreateDirectory(dirSave); 
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Photo.FileName); // Генеруємо унікальне ім'я файлу
                var saveFile = Path.Combine(dirSave, fileName); // Повний шлях до файлу
                using (var stream = new FileStream(saveFile, FileMode.Create)) // Зберігаємо файл
                {
                    model.Photo.CopyTo(stream); 
                }

                // Видалення старого зображення
                if (!string.IsNullOrEmpty(entity.Image)) // Якщо у категорії є зображення
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, "uploading", entity.Image); // Повний шлях до старого зображення
                    if (System.IO.File.Exists(oldImagePath)) // Перевірка чи існує файл
                    {
                        System.IO.File.Delete(oldImagePath); // Видаляємо файл
                    }
                }
                entity.Image = fileName; // Змінюємо шлях до зображення
            }

            _dbContext.SaveChanges(); // Зберігаємо зміни в базі даних
            return RedirectToAction("Index"); // Переходимо на головну сторінку
        }
    }
}
