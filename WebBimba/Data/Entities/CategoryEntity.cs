using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBimba.Data.Entities
{
    [Table("tbl_categories")]
    public class CategoryEntity //Entity - це клас який відповідає таблиці в базі даних
    {
        [Key] //Primary key
        public int Id { get; set; } //Id - це поле яке буде відповідати за ідентифікацію запису в таблиці
        [Required, StringLength(255)] //Required - поле обов'язкове для заповнення
        public string Name { get; set; } = String.Empty; //Name - назва категорії
        [StringLength(500)] //StringLength - максимальна довжина поля
        public string? Image { get; set; } //Image - шлях до зображення категорії
        [StringLength(4000)] //StringLength - максимальна довжина поля
        public string? Description { get; set; } //Description - опис категорії

        public virtual ICollection<ProductEntity>? ProductImages { get; set; } //ProductImages - колекція продуктів які відносяться до даної категорії
    }
}
