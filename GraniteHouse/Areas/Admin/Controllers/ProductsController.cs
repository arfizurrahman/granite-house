using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Areas.Admin.Controllers;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utilities;
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

            //Retrieve the newly saved product from database for saving image
            var productFromDb = _db.Products.Find(ProductVM.Product.Id);

            //Save image
            string webRootPath = _hostingEnvironment.WebRootPath;

            //retrieve all files
            var files = HttpContext.Request.Form.Files;

            //check if user has uploaded a file
            if (files.Count !=0)
            {
                //Image has been uploaded
                var uploads = Path.Combine(webRootPath, StaticDetails.ImageFolder);
                //Get the file extension
                var extension = Path.GetExtension(files[0].FileName);

                //Copy file from upload to server
                using (var filestream = new FileStream(Path.Combine(uploads, ProductVM.Product.Id + extension), FileMode.Create))
                {
                    //Save to file to server
                    files[0].CopyTo(filestream);
                }

                //Update the image column with the image path
                productFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductVM.Product.Id + extension;
            }
            else
            {
                //iF user doesn't select a file, add the default image path
                var uploads = Path.Combine(webRootPath,
                    StaticDetails.ImageFolder + @"\" + StaticDetails.DefaultProductImage);
                System.IO.File.Copy(uploads, webRootPath + @"\" + StaticDetails.ImageFolder + @"\" + ProductVM.Product.Id + ".jpeg");
                productFromDb.Image = @"\" + StaticDetails.ImageFolder + @"\" + ProductVM.Product.Id + ".jpeg";

            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        //GET : Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductVM.Product = await _db.Products.Include(m => m.ProductType).Include(m => m.SpecialTag)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ProductVM.Product == null)
            {
                return NotFound();
            }

            return View(ProductVM);
        }
    }
}