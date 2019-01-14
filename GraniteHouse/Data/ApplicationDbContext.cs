using System;
using System.Collections.Generic;
using System.Text;
using GraniteHouse.Models;
using GraniteHouse.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GraniteHouse.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<SpecialTag> SpecialTags { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ProductsSelectedForAppointment> ProductsSelectedForAppointments { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
} 
