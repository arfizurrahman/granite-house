using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Models.ViewModels
{
    public class AppointmentDetailsViewModel
    {
        public Appointment Appointment { get; set; }
        public List<Product> Products { get; set; }
        public List<ApplicationUser> SalesPersons { get; set; }
    }
}
