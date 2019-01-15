using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Data;
using GraniteHouse.Models;
using GraniteHouse.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraniteHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = StaticDetails.SuperAdminEndUser)]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.SpecialTags.ToList());
        }

        //Get Create Action Method
        public IActionResult Create()
        {
            return View();
        }

        //Poat Create Action Method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag specialTag)
        {
            if (ModelState.IsValid)
            {
                _db.SpecialTags.Add(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        //GET Edit action method
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        //POST edit action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SpecialTag specialTag)
        {
            if (id != specialTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _db.Update(specialTag);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialTag);
        }



        //GET Details action method
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        //GET Delete action method
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _db.SpecialTags.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        //Post Delete action method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var specialTag = await _db.SpecialTags.FindAsync(id);
            _db.SpecialTags.Remove(specialTag);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}