using Bogus;
using Microsoft.EntityFrameworkCore;
using WebBimba.Data;
using WebBimba.Data.Entities;
using WebBimba.Interfaces;
using WebBimba.Services;

var builder = WebApplication.CreateBuilder(args); //створюємо об'єкт для роботи з веб додатком

// Add services to the container.
builder.Services.AddDbContext<AppBimbaDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("MyConnectionDB"))); //додаємо сервіс для роботи з БД

builder.Services.AddScoped<IImageWorker, ImageWorker>(); //додаємо сервіс для роботи з зображеннями

builder.Services.AddControllersWithViews(); //додаємо сервіс для роботи з контролерами

var app = builder.Build(); //створюємо об'єкт для роботи з веб додатком

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) 
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles(); //дозволяє використовувати статичні файли

app.UseRouting(); //дозволяє використовувати маршрутизацію

app.UseAuthorization(); //дозволяє використовувати авторизацію

app.MapControllerRoute( //додаємо маршрут для контролерів
    name: "default",
    pattern: "{controller=Main}/{action=Index}/{id?}");

using (var serviceScope = app.Services.CreateScope()) //створюємо об'єкт для роботи з сервісами
{
    var context = serviceScope.ServiceProvider.GetService<AppBimbaDbContext>(); //отримуємо сервіс для роботи з БД
    var imageWorker = serviceScope.ServiceProvider.GetService<IImageWorker>(); //отримуємо сервіс для роботи з зображеннями

    // Apply migrations if they are not applied
    context.Database.Migrate(); //автоматичний запуск міграцій на БД, якщо їх там немає

    if (!context.Categories.Any()) //перевірка чи є категорії в БД
    {
        var imageName = imageWorker.Save("https://rivnepost.rv.ua/img/650/korisnoi-kovbasi-ne-buvae-hastroenterolohi-nazvali_20240612_4163.jpg");
        var kovbasa = new CategoryEntity
        {
            Name = "Ковбаси",
            Image = imageName,
            Description = "Тим часом відмовлятися від ковбаси повністю не обов’язково. " +
            "Важливо пам’ятати, що це делікатес, який можна вживати не більше 50 грамів на день."
        };

        imageName = imageWorker.Save("https://www.vsesmak.com.ua/sites/default/files/styles/large/public/field/image/syrnaya_gora_5330_1900_21.jpg?itok=RPUrRskl");
        var cheese = new CategoryEntity
        {
            Name = "Сири",
            Image = imageName,
            Description = "Cир – один з найчастіших гостей на нашому столі. " +
            "Адже це і смачно, і корисно, і доступно. Не можна сказати, що увесь, " +
            "що продається на прилавках супермаркетів твердий сир – неякісний."
        };

        imageName = imageWorker.Save("https://upload.wikimedia.org/wikipedia/commons/thumb/7/7b/Assorted_bread.jpg/420px-Assorted_bread.jpg");
        var bread = new CategoryEntity
        {
            Name = "Хліб",
            Image = imageName,
            Description = "У сегменті ринку «здорового харчування» існують сорти хліба, " +
            "які майже не сприяють набору зайвої ваги – наприклад, цільнозерновий хліб."
        };

        context.Categories.Add(kovbasa);
        context.Categories.Add(cheese);
        context.Categories.Add(bread);
        context.SaveChanges();
    }

    //перевірка чи є продукти в БД
    if (!context.Products.Any()) 
    {
        var categories = context.Categories.ToList(); //отримуємо всі категорії з БД

        var fakerProduct = new Faker<ProductEntity>("uk")  // генератор даних для продуктів
                    .RuleFor(u => u.Name, (f, u) => f.Commerce.Product()) 
                    .RuleFor(u => u.Price, (f, u) => decimal.Parse(f.Commerce.Price()))
                    .RuleFor(u => u.Category, (f, u) => f.PickRandom(categories)); 

        string url = "https://picsum.photos/1200/800?product"; //посилання на зображення

        var products = fakerProduct.GenerateLazy(30); //генеруємо 30 продуктів

        Random r = new Random(); //генератор випадкових чисел

        foreach (var product in products) //додаємо продукти в БД
        {
            context.Add(product); //додаємо продукт в контекст
            context.SaveChanges(); //зберігаємо зміни в БД
            int imageCount = r.Next(3, 5); //генеруємо випадкову кількість зображень
            for (int i = 0; i < imageCount; i++) //додаємо зображення для продуктів
            {
                var imageName = imageWorker.Save(url); //зберігаємо зображення
                var imageProduct = new ProductImageEntity //створюємо об'єкт для зображення
                {
                    Product = product, 
                    Image = imageName,
                    Priority = i
                };
                context.Add(imageProduct); //додаємо зображення в контекст
                context.SaveChanges(); //зберігаємо зміни в БД
            }
        }
    }
}

app.Run(); //запускаємо додаток
