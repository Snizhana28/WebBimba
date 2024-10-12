using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBimba.Data.Entities
{
    [Table("tbl_products")]
    public class ProductEntity //Entity - це клас який відповідає таблиці в базі даних
    {
        [Key] //Primary key
        public int Id { get; set; } //Id - це поле яке буде відповідати за ідентифікацію запису в таблиці
        [Required, StringLength(255)] //Required - поле обов'язкове для заповнення
        public string Name { get; set; } = String.Empty; //Name - назва продукту
        public decimal Price { get; set; } //Price - ціна продукту
        [ForeignKey("Category")] //ForeignKey - зовнішній ключ
        public int CategoryId { get; set; } //CategoryId - це зовнішній ключ який вказує на категорію до якої відноситься продукт
        public CategoryEntity? Category { get; set; }   //Category - категорія до якої відноситься продукт
        public virtual ICollection<ProductImageEntity>? ProductImages { get; set; } //ProductImages - колекція зображень продукту
    }
}