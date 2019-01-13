using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Extensions;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartVM { get; set; }

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCartVM = new ShoppingCartViewModel
            {
                Products = new List<Product>()
            };
        }

        // GET : Index 
        public async Task<IActionResult> Index()
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>(StaticDetails.ShoppingCartName);

            if (shoppingCartList.Count > 0)
            {
                foreach (var cartItem in shoppingCartList)
                {
                    Product product = _db.Products
                        .Include(p=>p.ProductType)
                        .Include(p=>p.SpecialTag)
                        .Where(p => p.Id == cartItem).FirstOrDefault();

                    ShoppingCartVM.Products.Add(product);
                }
                
            }
            return View(ShoppingCartVM);
        }
    }
}