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

        // POST : Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            List<int> cartItemList = HttpContext.Session.Get<List<int>>(StaticDetails.ShoppingCartName);

            ShoppingCartVM.Appointment.AppointmentDate = ShoppingCartVM.Appointment.AppointmentDate
                .AddHours(ShoppingCartVM.Appointment.AppointmentTime.Hour)
                .AddMinutes(ShoppingCartVM.Appointment.AppointmentTime.Minute);

            Appointment appointment = ShoppingCartVM.Appointment;

            _db.Appointments.Add(appointment);
            _db.SaveChanges();

            int appointmentId = appointment.Id;

            foreach (var productId in cartItemList)
            {
                ProductsSelectedForAppointment productsSelected = new ProductsSelectedForAppointment()
                {
                    AppointmentId = appointmentId,
                    ProductId = productId
                };

                _db.ProductsSelectedForAppointments.Add(productsSelected);
            }

            _db.SaveChanges();
            cartItemList = new List<int>();
            HttpContext.Session.Set(StaticDetails.ShoppingCartName, cartItemList);

            return RedirectToAction("Index");

        }

        public IActionResult Remove(int id)
        {
            List<int> shoppingCartList = HttpContext.Session.Get<List<int>>(StaticDetails.ShoppingCartName);

            if (shoppingCartList.Count > 0)
            {
                if (shoppingCartList.Contains(id))
                {
                    shoppingCartList.Remove(id);
                }
            }

            HttpContext.Session.Set(StaticDetails.ShoppingCartName, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

    }
}