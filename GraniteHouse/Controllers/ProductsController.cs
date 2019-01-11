using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Areas.Admin.Controllers;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ProductViewModel ProductVM { get; set; }

        public ProductsController(ApplicationDbContext db)
        {
            _db = db;
            ProductVM = new ProductViewModel()
            {
                ProductTypes = _db.ProductTypes.ToList(),
                SpecialTags = _db.SpecialTags.ToList(),
                Product = new Product()

            };
        }
        public async Task<IActionResult> Index()
        {
            var products = _db.Products.Include(m=>m.ProductType).Include(m=>m.SpecialTag);
            return View(await products.ToListAsync());
        }
    }
}