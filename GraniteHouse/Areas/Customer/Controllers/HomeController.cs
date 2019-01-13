using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using Microsoft.AspNetCore.Mvc;
using GraniteHouse.Models;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var productList = await _db.Products
                .Include(m => m.ProductType)
                .Include(m => m.SpecialTag)
                .ToListAsync();
            return View(productList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products
                .Include(m=>m.ProductType)
                .Include(m=>m.SpecialTag)
                .Where(m=>m.Id == id)
                .FirstOrDefaultAsync();

            return View(product);
        }

        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
