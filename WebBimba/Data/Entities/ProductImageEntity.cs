using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBimba.Data.Entities
{
    [Table("tbl_product_images")]
    public class ProductImageEntity //ProductImageEntity - це клас який відповідає таблиці в базі даних
    {
        [Key] //Primary key
        public int Id { get; set; } //Id - це поле яке буде відповідати за ідентифікацію запису в таблиці
        [Required, StringLength(255)] //Required - поле обов'язкове для заповнення
        public string Image { get; set; } = string.Empty; //Image - шлях до зображення продукту
        public int Priority { get; set; } //Priority - пріоритет зображення
        [ForeignKey("Product")] //ForeignKey - зовнішній ключ
        public int ProductId { get; set; } //ProductId - це зовнішній ключ який вказує на продукт до якого відноситься зображення
        public virtual ProductEntity? Product { get; set; } //Product - продукт до якого відноситься зображення
    }
}