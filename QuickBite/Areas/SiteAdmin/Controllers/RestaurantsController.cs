﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuickBite.Data;
using QuickBite.Models;

namespace QuickBite.Areas.SiteAdmin.Controllers
{
    [Area("SiteAdmin")]
    public class RestaurantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RestaurantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Restaurant.ToListAsync());
        }

        // GET: SiteAdmin/Restaurants/Create
        public IActionResult Create()
        {
            // Populate a dropdown with users
            ViewData["RestaurantOwners"] = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FirstName} {u.LastName}" // Combine first and last name
                })
                .ToList();

            return View();
        }

        // POST: SiteAdmin/Restaurants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RestaurantId,Name,Description,Photo,RestaurantOwenrId")] QuickBite.Models.Restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                restaurant.RestaurantId = Guid.NewGuid();
                restaurant.isAccepted = true;
                _context.Add(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdown in case of errors
            ViewData["RestaurantOwners"] = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FirstName} {u.LastName}"
                })
                .ToList();

            return View(restaurant);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Restaurant
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            // Populate RestaurantOwners for dropdown
            ViewBag.RestaurantOwners = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FirstName} {u.LastName}"
                })
                .ToList();

            return View(restaurant);
        }

        // POST: SiteAdmin/Restaurants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("RestaurantId,Name,Description,Photo,Address,Latitude,Longitude,DeliveryRadius,OpeningHour,CloseingHour,isAccepted,RestaurantOwenrId")] QuickBite.Models.Restaurant restaurant)
        {
            if (id != restaurant.RestaurantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    restaurant.OpeningHour = restaurant.OpeningHour.ToUniversalTime();
                    restaurant.CloseingHour = restaurant.CloseingHour.ToUniversalTime();
                    _context.Update(restaurant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RestaurantExists(restaurant.RestaurantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // Repopulate RestaurantOwners for dropdown in case of errors
            ViewBag.RestaurantOwners = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FirstName} {u.LastName}"
                })
                .ToList();

            return View(restaurant);
        }
        // GET: SiteAdmin/Restaurants/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurant
                .FirstOrDefaultAsync(m => m.RestaurantId == id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return View(restaurant);
        }

        // POST: SiteAdmin/Restaurants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var restaurant = await _context.Restaurant.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurant.Remove(restaurant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RestaurantExists(Guid id)
        {
            return _context.Restaurant.Any(e => e.RestaurantId == id);
        }
    }
}
