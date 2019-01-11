using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Areas.Admin.Controllers;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ProductViewModel ProductVM { get; set; }

        public ProductsController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
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

        //GET: Products create
        public IActionResult Create()
        {
            return View(ProductVM);
        }

        //POST : Products Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            if (!ModelState.IsValid)
            {
                return View(ProductVM);
            }

            _db.Products.Add(ProductVM.Product);
            await _db.SaveChangesAsync();

            //Save image
            string webRootPath = _hostingEnvironment.WebRootPath;

        }
    }
}