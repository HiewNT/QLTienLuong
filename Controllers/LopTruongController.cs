using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using QLTienLuong.Models;
using System.Security.Claims;

namespace QLTienLuong.Controllers
{
    [LopTruongOnly]
    public class LopTruongController : Controller
    {
        private readonly QltienLuongContext _context;

        public LopTruongController(QltienLuongContext context)
        {
            _context = context;
        }

        // GET: LopTruong/Index
        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .Include(u => u.MaHocVienNavigation.MaQuocTichNavigation)
                .FirstOrDefaultAsync(u => u.MaUser == userId);

            if (user?.MaHocVienNavigation?.MaQuocTich == null)
            {
                return NotFound("Không tìm thấy thông tin quốc tịch của lớp trưởng");
            }

            // Lấy danh sách học viên cùng quốc tịch với lớp trưởng
            var hocViens = await _context.HocViens
                .Include(h => h.MaQuocTichNavigation)
                .Include(h => h.PhuCapHocViens)
                .Include(h => h.KhenThuongKyLuats)
                .Include(h => h.HocVienTangLuongs)
                    .ThenInclude(htl => htl.MaTangLuongNavigation)
                .Where(h => h.MaQuocTich == user.MaHocVienNavigation.MaQuocTich)
                .OrderBy(h => h.HoTen)
                .ToListAsync();

            ViewBag.QuocTich = user.MaHocVienNavigation.MaQuocTichNavigation.TenQuocTich;
            ViewBag.LopTruong = user.MaHocVienNavigation.HoTen;

            return View(hocViens);
        }

        // GET: LopTruong/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .FirstOrDefaultAsync(u => u.MaUser == userId);

            if (user?.MaHocVienNavigation?.MaQuocTich == null)
            {
                return NotFound("Không tìm thấy thông tin quốc tịch của lớp trưởng");
            }

            var hocVien = await _context.HocViens
                .Include(h => h.MaQuocTichNavigation)
                .Include(h => h.PhuCapHocViens)
                    .ThenInclude(pc => pc.MaPhuCapNavigation)
                .Include(h => h.KhenThuongKyLuats)
                .Include(h => h.HocVienTangLuongs)
                    .ThenInclude(htl => htl.MaTangLuongNavigation)
                .Include(h => h.HuongPhuCaps)
                .FirstOrDefaultAsync(h => h.MaHocVien == id && h.MaQuocTich == user.MaHocVienNavigation.MaQuocTich);

            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }
    }
}
