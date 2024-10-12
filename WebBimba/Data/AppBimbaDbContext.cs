using Microsoft.EntityFrameworkCore;
using WebBimba.Data.Entities;

namespace WebBimba.Data
{
    public class AppBimbaDbContext : DbContext //AppBimbaDbContext - це клас який відповідає за зв'язок з базою даних
    {
        public AppBimbaDbContext(DbContextOptions<AppBimbaDbContext> options) // конструктор класу AppBimbaDbContext
            : base(options) { }

        public DbSet<CategoryEntity> Categories { get; set; } //Categories - це властивість яка вказує на таблицю категорій
        public DbSet<ProductEntity> Products { get; set; } //Products - це властивість яка вказує на таблицю продуктів
        public DbSet<ProductImageEntity> ProductImages { get; set; } //ProductImages - це властивість яка вказує на таблицю зображень продуктів


    }
}
