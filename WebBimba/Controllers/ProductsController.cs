using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using WebBimba.Data;
using WebBimba.Models.Product;

namespace WebBimba.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppBimbaDbContext _dbContext;
        private readonly IMapper _mapper;
        //DI - Depencecy Injection
        public ProductsController(AppBimbaDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            List<ProductItemViewModel> model = _dbContext.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return View(model);
        }
        public IActionResult Search(string? name, decimal? minPrice, decimal? maxPrice)
        {
            // Ініціалізуємо запит до БД
            var query = _dbContext.Products.AsQueryable();

            // Якщо введено назву, то шукаємо по ній
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }

            // Якщо введено мінімальну ціну, то шукаємо по ній
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            // Якщо введено максимальну ціну, то шукаємо по ній
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Проектуємо результат пошуку на модель
            var model = query
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();

            return View("Index", model); // Повертаємо результат пошуку у подання Index
        }
    }
}
