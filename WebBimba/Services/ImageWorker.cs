using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using WebBimba.Interfaces;

namespace WebBimba.Services
{
    public class ImageWorker : IImageWorker //реалізація інтерфейсу
    {
        private readonly IWebHostEnvironment _environment; //змінна для роботи з середовищем
        private const string dirName = "uploading"; //назва папки для збереження файлів
        private int[] sizes = [50, 150, 300, 600, 1200]; //розміри зображень
        public ImageWorker(IWebHostEnvironment environment) //конструктор
        {
            _environment = environment; //ініціалізуємо середовище
        }
        public string Save(string url) // метод для збереження файлу
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Send a GET request to the image URL
                    HttpResponseMessage response = client.GetAsync(url).Result;

                    // Check if the response status code indicates success (e.g., 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the image bytes from the response content
                        byte[] imageBytes = response.Content.ReadAsByteArrayAsync().Result;
                        return CompresImage(imageBytes); // Save the image
                    }
                    else
                    {
                        Console.WriteLine($"Failed to retrieve image. Status code: {response.StatusCode}"); // Log the error
                        return String.Empty; // Return an empty string
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}"); // Log the error
                return String.Empty; // Return an empty string
            }
        }

        /// <summary>
        /// Стискаємо фото
        /// </summary>
        /// <param name="bytes">Набір байтів фото</param>
        /// <returns>Повертаємо назву збереженого фото</returns>
        private string CompresImage(byte[] bytes) // метод  для стискання зображення
        {
            string imageName = Guid.NewGuid().ToString() + ".webp"; //створюємо унікальне ім'я файлу

            var dirSave = Path.Combine(_environment.WebRootPath, dirName); //повний шлях до папки
            if (!Directory.Exists(dirSave)) // перевірка чи існує папка
            {
                Directory.CreateDirectory(dirSave); //створюємо папку
            }
            
            foreach (int size in sizes)  // цикл для стискання зображення
            {
                var path = Path.Combine(dirSave, $"{size}_{imageName}"); //повний шлях до файлу
                using (var image = Image.Load(bytes)) // завантажуємо зображення
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(size, size),
                        Mode = ResizeMode.Max
                    })); // Resize the image
                    image.SaveAsWebp(path); // Save the resized image
                    //image.Save(path, new WebpEncoder()); // Save the resized image
                }
            }

            return imageName;
        }

        public void Delete(string fileName) // метод для видалення файлу
        {
            var fileSave = Path.Combine(_environment.WebRootPath, dirName, fileName); //повний шлях до файлу
            if (File.Exists(fileSave)) // перевірка чи файл існує
                File.Delete(fileSave); // видаляємо файл
        }
        public string Save(IFormFile file) // метод для збереження файлу
        {
            try
            {
                using (var memoryStream = new MemoryStream()) //створюємо потік пам'яті
                {
                    file.CopyTo(memoryStream); //копіюємо файл в потік пам'яті
                    byte[] imageBytes = memoryStream.ToArray(); //читаємо дані з потоку пам'яті
                    return CompresImage(imageBytes); //зберігаємо файл
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}"); //логуємо помилку
                return String.Empty; //повертаємо пустий рядок
            }
        }
    }
}
