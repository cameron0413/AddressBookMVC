﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using USETHISAddressBookMVC.Data;
using USETHISAddressBookMVC.Models;
using USETHISAddressBookMVC.Models.ViewModels;
using USETHISAddressBookMVC.Services.Interfaces;

namespace USETHISAddressBookMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IABEmailService _emailService;

        public CategoriesController(ApplicationDbContext context,
                                    UserManager<AppUser> userManager,
                                    IABEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: Categories
        [Authorize]

        public async Task<IActionResult> Index(string swalMessage = null)
        {
            ViewData["SwalMessage"] = swalMessage;
            string appUserId = _userManager.GetUserId(User);
            List<Category> categories = await _context.Category!
                                                      .Where(c => c.AppUserId == appUserId)
                                                      .OrderBy(c => c.Name)
                                                      .ToListAsync();

            return View(categories);
        }

        // GET: Categories/Details/5
        //Deleted

        // GET: Categories/Create
        [Authorize]
        public IActionResult Create()
        {

            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Category category)
        {
            ModelState.Remove("AppUserId");

            string appUserId = _userManager.GetUserId(User);


            if (ModelState.IsValid)
            {
                category.AppUserId = appUserId;
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", category.AppUserId);
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string appUserId = _userManager.GetUserId(User);

            var category = await _context.Category
                                   .Where(c => c.Id == id && c.AppUserId == appUserId)
                                   .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string appUserId = _userManager.GetUserId(User);

                    category.AppUserId = appUserId;

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", category.AppUserId);
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Category == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Category'  is null.");
            }
            var category = await _context.Category.FindAsync(id);
            if (category != null)
            {
                _context.Category.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return (_context.Category?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize]
        public async Task<IActionResult> EmailCategory(int id)
        {
            string appUserId = _userManager.GetUserId(User);
            Category category = await _context.Category
                                              .Include(c => c.Contacts)
                                              .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == appUserId);

            List<string> emails = category.Contacts.Select(c => c.Email).ToList();

            EmailData emailData = new EmailData()
            {
                GroupName = category.Name,
                EmailAddress = String.Join(";", emails),
                Subject = $"GroupMessage For {category.Name} Group"
            };

            EmailCategoryViewModel model = new()
            {
                Contacts = category.Contacts.ToList(),
                EmailData = emailData,

            };

            return View(model);

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EmailCategory(EmailCategoryViewModel ecvm)
        {
            if (ModelState.IsValid)
            {
               //appUserId line deleted
                string htmlMessage = ecvm.EmailData.Body;
                await _emailService.SendEmailAsync(ecvm.EmailData.EmailAddress, ecvm.EmailData.Subject, htmlMessage);

                return RedirectToAction("Index", "Categories");
            }

            return View();
        }


    }
}
