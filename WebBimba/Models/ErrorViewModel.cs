namespace WebBimba.Models
{
    public class ErrorViewModel // Клас для відображення помилок
    {
        public string? RequestId { get; set; } // RequestId - ідентифікатор запиту

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // Показувати ідентифікатор запиту
    }
}
