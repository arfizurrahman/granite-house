using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using GraniteHouse.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.SuperAdminEndUser + "," + StaticDetails.AdminEndUser)]
    [Area("Admin")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int itemsPerPage = 3;

        public AppointmentsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index(int productPage=1, string searchName = null, string searchEmail = null, string searchPhone = null, string searchDate = null)
        {
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVM = new AppointmentViewModel()
            {
                Appointments = new List<Appointment>()
            };

            StringBuilder param = new StringBuilder();

            param.Append("/Admin/Appointments?productPage=:");
            param.Append("&searchName=");
            if (searchName != null)
            {
                param.Append(searchName);
            }

            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }

            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }

            param.Append("&searchDate=");
            if (searchDate != null)
            {
                param.Append(searchDate);
            }

            appointmentVM.Appointments = _db.Appointments.Include(u => u.SalesPerson).ToList();
            if (User.IsInRole(StaticDetails.AdminEndUser))
            {
                appointmentVM.Appointments = appointmentVM.Appointments.Where(a => a.SalesPersonId == claim.Value).ToList();
            }

            if (searchName != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments
                    .Where(a => a.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }
            if (searchEmail != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments
                    .Where(a => a.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }
            if (searchPhone != null)
            {
                appointmentVM.Appointments = appointmentVM.Appointments
                    .Where(a => a.CustomerPhoneNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }
            if (searchDate != null)
            {
                try
                {
                    DateTime appDate = Convert.ToDateTime(searchDate);
                    ; appointmentVM.Appointments = appointmentVM.Appointments
                         .Where(a => a.AppointmentDate.ToShortDateString().Equals(appDate.ToShortDateString())).ToList();
                }
                catch (Exception e)
                {

                }
            }

            var count = appointmentVM.Appointments.Count;

            appointmentVM.Appointments = appointmentVM.Appointments.OrderBy(a => a.AppointmentDate)
                .Skip((productPage - 1) * itemsPerPage)
                .Take(itemsPerPage).ToList();

            appointmentVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = productPage,
                ItemsPerPage =  itemsPerPage,
                TotalItems = count,
                UrlParam = param.ToString()
            };


            return View(appointmentVM);
        }

        //GET : Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Product>)(from p in _db.Products
                                                  join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                                                  where a.AppointmentId == id
                                                  select p).Include("ProductType");
            AppointmentDetailsViewModel appointmentDetailsViewModel = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointments.Where(a => a.Id == id).Include(a => a.SalesPerson).FirstOrDefaultAsync(),
                SalesPersons = await _db.ApplicationUsers.ToListAsync(),
                Products = productList.ToList()
            };

            return View(appointmentDetailsViewModel);
        }

        //POST : Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AppointmentDetailsViewModel appointmentDetails)
        {
            if (ModelState.IsValid)
            {
                appointmentDetails.Appointment.AppointmentDate = appointmentDetails.Appointment.AppointmentDate
                    .AddHours(appointmentDetails.Appointment.AppointmentTime.Hour)
                    .AddMinutes(appointmentDetails.Appointment.AppointmentTime.Minute);

                var appointmentFromDb =
                    _db.Appointments.FirstOrDefault(a => a.Id == appointmentDetails.Appointment.Id);

                appointmentFromDb.CustomerName = appointmentDetails.Appointment.CustomerName;
                appointmentFromDb.CustomerEmail = appointmentDetails.Appointment.CustomerEmail;
                appointmentFromDb.CustomerPhoneNumber = appointmentDetails.Appointment.CustomerPhoneNumber;
                appointmentFromDb.AppointmentDate = appointmentDetails.Appointment.AppointmentDate;
                appointmentFromDb.IsConfirmed = appointmentDetails.Appointment.IsConfirmed;

                if (User.IsInRole(StaticDetails.SuperAdminEndUser))
                {
                    appointmentFromDb.SalesPersonId = appointmentDetails.Appointment.SalesPersonId;
                }

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(appointmentDetails);
        }

        //GET : Edit
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Product>)(from p in _db.Products
                join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductType");
            AppointmentDetailsViewModel appointmentDetailsViewModel = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointments.Where(a => a.Id == id).Include(a => a.SalesPerson).FirstOrDefaultAsync(),
                SalesPersons = await _db.ApplicationUsers.ToListAsync(),
                Products = productList.ToList()
            };

            return View(appointmentDetailsViewModel);
        }

        //GET : Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var productList = (IEnumerable<Product>)(from p in _db.Products
                join a in _db.ProductsSelectedForAppointments on p.Id equals a.ProductId
                where a.AppointmentId == id
                select p).Include("ProductType");
            AppointmentDetailsViewModel appointmentDetailsViewModel = new AppointmentDetailsViewModel()
            {
                Appointment = await _db.Appointments.Where(a => a.Id == id).Include(a => a.SalesPerson).FirstOrDefaultAsync(),
                SalesPersons = await _db.ApplicationUsers.ToListAsync(),
                Products = productList.ToList()
            };

            return View(appointmentDetailsViewModel);
        }

        //POST : Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            _db.Appointments.Remove(appointment);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}