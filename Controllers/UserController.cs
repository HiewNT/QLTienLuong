using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QLTienLuong.Models;

using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System;

namespace QLTienLuong.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly QltienLuongContext _context;

        public UserController(QltienLuongContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .Include(u => u.MaRoleNavigation)
                .ToListAsync();
            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .Include(u => u.MaRoleNavigation)
                .FirstOrDefaultAsync(m => m.MaUser == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public async Task<IActionResult> Create()
        {
            // Populate dropdowns
            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien");
            ViewData["MaRole"] = new SelectList(_context.Roles, "MaRole", "TenRole");
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,PasswordHash,MaHocVien,MaRole,Active")] UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra username đã tồn tại chưa
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại!");
                    ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", model.MaHocVien);
                    ViewData["MaRole"] = new SelectList(_context.Roles, "MaRole", "TenRole", model.MaRole);
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = HashPassword(model.Password),
                    MaHocVien = model.MaHocVien,
                    MaRole = model.MaRole,
                    Active = model.Active
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", model.MaHocVien);
            ViewData["MaRole"] = new SelectList(_context.Roles, "MaRole", "TenRole", model.MaRole);
            return View(model);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new UserEditViewModel
            {
                MaUser = user.MaUser,
                Username = user.Username,
                MaHocVien = user.MaHocVien,
                MaRole = user.MaRole,
                Active = user.Active ?? true
            };

            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", model.MaHocVien);
            ViewData["MaRole"] = new SelectList(_context.Roles, "MaRole", "TenRole", model.MaRole);
            return View(model);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaUser,Username,MaHocVien,MaRole,Active")] UserEditViewModel model)
        {
            if (id != model.MaUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Kiểm tra username đã tồn tại chưa (trừ user hiện tại)
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username && u.MaUser != id);
                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại!");
                        ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", model.MaHocVien);
                        ViewData["MaRole"] = new SelectList(_context.Roles, "MaRole", "TenRole", model.MaRole);
                        return View(model);
                    }

                    user.Username = model.Username;
                    user.MaHocVien = model.MaHocVien;
                    user.MaRole = model.MaRole;
                    user.Active = model.Active;

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.MaUser))
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
            ViewData["MaHocVien"] = new SelectList(_context.HocViens, "MaHocVien", "MaHocVien", model.MaHocVien);
            ViewData["MaRole"] = new SelectList(_context.Roles, "MaRole", "TenRole", model.MaRole);
            return View(model);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .Include(u => u.MaRoleNavigation)
                .FirstOrDefaultAsync(m => m.MaUser == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: User/ToggleActive
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return Json(new { success = false });
                }

                // Toggle trạng thái
                user.Active = !(user.Active ?? false);
                _context.Update(user);
                await _context.SaveChangesAsync();

                return Json(new { 
                    success = true, 
                    active = user.Active ?? false
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.MaUser == id);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
