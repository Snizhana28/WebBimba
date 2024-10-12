namespace WebBimba.Interfaces
{
    public interface IImageWorker //інтерфейс для роботи з зображеннями
    {
        string Save(string url); //метод для збереження 

        string Save(IFormFile file); //метод для збереження   
        void Delete(string fileName); //метод для видалення
    }
}
