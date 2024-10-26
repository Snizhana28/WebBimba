using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBimba.Data;
using WebBimba.Data.Entities;
using WebBimba.Interfaces;
using WebBimba.Models.Product;

namespace WebBimba.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppBimbaDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageWorker _imageWorker;
        private readonly IWebHostEnvironment _environment;
        //DI - Depencecy Injection
        public ProductsController(AppBimbaDbContext context, IMapper mapper, IImageWorker imageWorker,
            IWebHostEnvironment environment)
        {
            _dbContext = context;
            _mapper = mapper;
            _imageWorker = imageWorker;
            _environment = environment;
        }
        public IActionResult Index()
        {
            List<ProductItemViewModel> model = _dbContext.Products
                .ProjectTo<ProductItemViewModel>(_mapper.ConfigurationProvider)
                .ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.Include(x => x.ProductImages).SingleOrDefault(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            if (product.ProductImages != null)
            {
                foreach (var productImage in product.ProductImages)
                {
                    _imageWorker.Delete(productImage.Image);
                    _dbContext.ProductImages.Remove(productImage);

                }
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();

            return Json(new { text = "Ми його видалили" }); // Вертаю об'єкт у відповідь
        }


        [HttpGet]
        public IActionResult Create()
        {
            var categories = _dbContext.Categories
                .Select(x => new { Value = x.Id, Text = x.Name })
                .ToList();

            ProductCreateViewModel viewModel = new()
            {
                CategoryList = new SelectList(categories, "Value", "Text")
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Create(ProductCreateViewModel model)
        {
            var entity = _mapper.Map<ProductEntity>(model);
            // Збереження в Базу даних інформації   
            var dirName = "uploading";
            var dirSave = Path.Combine(_environment.WebRootPath, dirName);

            if (!Directory.Exists(dirSave))
            {
                Directory.CreateDirectory(dirSave);
            }

            entity.ProductImages = new List<ProductImageEntity>(); // Ініціалізуємо колекцію

            if (model.Photos != null && model.Photos.Count > 0)
            {
                int priority = 0;
                // Збереження фотографій
                foreach (var photo in model.Photos)
                {
                    if (photo.Length > 0)
                    {
                        var productImageEntity = new ProductImageEntity()
                        {
                            Product = entity,
                            Image = _imageWorker.Save(photo),
                            Priority = priority++
                        };
                        entity.ProductImages.Add(productImageEntity); // Додаємо до колекції

                    }
                }
            }
            _dbContext.Products.Add(entity);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _dbContext.Products.Find(id);
            var model = new ProductEditViewModel
            {
                Id = id,
                Name = product.Name,
                Price = product.Price
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(ProductEditViewModel model)
        {
            var entity = _mapper.Map<ProductEntity>(model);
            var product = _dbContext.Products.Find(model.Id);

            if (product == null)
            {
                return NotFound();
            }
            var productImg = _dbContext.ProductImages.Where(p => p.ProductId == product.Id).ToList();

            foreach (var img in productImg)
            {
                if (!string.IsNullOrEmpty(img.Image))
                {
                    _imageWorker.Delete(img.Image);
                    _dbContext.ProductImages.Remove(img);
                }
            }
            product.Price = entity.Price;
            product.Name = entity.Name;

            int imageCount = model.Photos.Count();
            for (int i = 0; i < imageCount; i++)
            {
                var imageProduct = new ProductImageEntity
                {
                    Product = product,
                    Image = _imageWorker.Save(model.Photos.ElementAt(i)),
                    Priority = i
                };
                _dbContext.Add(imageProduct);
                _dbContext.SaveChanges();
            }
            return Redirect("/");
        }

        public IActionResult Details(int id)
        {
            var product = _dbContext.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<ProductItemViewModel>(product);
            return View(model);
        }
    }

}

