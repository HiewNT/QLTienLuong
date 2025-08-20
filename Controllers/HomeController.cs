using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTienLuong.Models;

namespace QLTienLuong.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly QltienLuongContext _context;

        public HomeController(ILogger<HomeController> logger, QltienLuongContext context)
        {
            _logger = logger;
            _context = context;
        }

        [AdminOrNhanVienTaiChinh]
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = new DashboardViewModel
                {
                    TotalHocVien = await _context.HocViens.CountAsync(),
                    TotalKhenThuong = await _context.KhenThuongKyLuats.CountAsync(),
                    TotalPhuCap = await _context.PhuCapHocViens.CountAsync(),
                    TotalTangLuong = await _context.TangLuongs.CountAsync(),
                    TotalQuocTich = await _context.QuocTiches.CountAsync(),
                    TotalUser = await _context.Users.CountAsync(),
                    
                    // Thống kê học viên theo quốc tịch
                    HocVienTheoQuocTich = await _context.HocViens
                        .Include(h => h.MaQuocTichNavigation)
                        .GroupBy(h => h.MaQuocTichNavigation.TenQuocTich)
                        .Select(g => new HocVienQuocTichStat
                        {
                            TenQuocTich = g.Key,
                            SoLuong = g.Count()
                        })
                        .Take(5)
                        .ToListAsync(),
                    
                    // Thống kê khen thưởng kỷ luật theo loại
                    KhenThuongKyLuatTheoLoai = await _context.KhenThuongKyLuats
                        .GroupBy(k => k.LoaiHinh)
                        .Select(g => new KhenThuongKyLuatStat
                        {
                            LoaiHinh = g.Key,
                            SoLuong = g.Count(),
                            TongTien = g.Sum(k => k.SoTien ?? 0)
                        })
                        .ToListAsync(),
                    
                    // Học viên mới nhất
                    HocVienMoiNhat = await _context.HocViens
                        .Include(h => h.MaQuocTichNavigation)
                        .OrderByDescending(h => h.MaHocVien)
                        .Take(5)
                        .ToListAsync(),
                    
                    // Khen thưởng gần đây
                    KhenThuongGanDay = await _context.KhenThuongKyLuats
                        .Include(k => k.MaHocVienNavigation)
                        .OrderByDescending(k => k.NgayQuyetDinh)
                        .Take(5)
                        .ToListAsync()
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải dữ liệu dashboard");
                return View(new DashboardViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // GET: Home/Profile
        [AdminOrHocVienOrLopTruong]
        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("Login", "Account"); }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .Include(u => u.MaRoleNavigation)
                .Include(u => u.MaHocVienNavigation.MaQuocTichNavigation)
                .FirstOrDefaultAsync(u => u.MaUser == userId);
            if (user == null) { return NotFound(); }

            // Load thông tin phụ cấp đang nhận
            var phuCapHocViens = new List<PhuCapHocVien>();
            if (!string.IsNullOrEmpty(user.MaHocVien))
            {
                phuCapHocViens = await _context.PhuCapHocViens
                    .Include(p => p.MaPhuCapNavigation)
                    .Where(p => p.MaHocVien == user.MaHocVien)
                    .ToListAsync();
            }

            // Load thông tin khen thưởng kỷ luật
            var khenThuongKyLuats = new List<KhenThuongKyLuat>();
            if (!string.IsNullOrEmpty(user.MaHocVien))
            {
                khenThuongKyLuats = await _context.KhenThuongKyLuats
                    .Where(k => k.MaHocVien == user.MaHocVien)
                    .OrderByDescending(k => k.NgayQuyetDinh)
                    .ToListAsync();
            }

            // Load thông tin tăng cường
            var hocVienTangLuongs = new List<HocVienTangLuong>();
            if (!string.IsNullOrEmpty(user.MaHocVien))
            {
                hocVienTangLuongs = await _context.HocVienTangLuongs
                    .Include(h => h.MaTangLuongNavigation)
                    .Where(h => h.MaHocVien == user.MaHocVien)
                    .OrderByDescending(h => h.ThangNam)
                    .ToListAsync();
            }

            // Load thông tin hưởng phụ cấp gần đây
            var huongPhuCaps = new List<HuongPhuCap>();
            if (!string.IsNullOrEmpty(user.MaHocVien))
            {
                huongPhuCaps = await _context.HuongPhuCaps
                    .Where(h => h.MaHocVien == user.MaHocVien)
                    .OrderByDescending(h => h.ThangNam)
                    .Take(5)
                    .ToListAsync();
            }

            var profileViewModel = new ProfileViewModel
            {
                User = user,
                PhuCapHocViens = phuCapHocViens,
                KhenThuongKyLuats = khenThuongKyLuats,
                HocVienTangLuongs = hocVienTangLuongs,
                HuongPhuCaps = huongPhuCaps
            };

            return View(profileViewModel);
        }

        // GET: Home/EditProfile
        [AdminOrHocVienOrLopTruong]
        public async Task<IActionResult> EditProfile()
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("Login", "Account"); }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .Include(u => u.MaHocVienNavigation.MaQuocTichNavigation)
                .FirstOrDefaultAsync(u => u.MaUser == userId);
            if (user == null) { return NotFound(); }

            var model = new EditProfileViewModel
            {
                MaUser = user.MaUser,
                Username = user.Username,
                MaHocVien = user.MaHocVien,
                HoTen = user.MaHocVienNavigation?.HoTen,
                NgaySinh = user.MaHocVienNavigation?.NgaySinh,
                Khoa = user.MaHocVienNavigation?.Khoa,
                Lop = user.MaHocVienNavigation?.Lop,
                Nganh = user.MaHocVienNavigation?.Nganh,
                DonVi = user.MaHocVienNavigation?.DonVi,
                MaQuocTich = user.MaHocVienNavigation?.MaQuocTich,
                NamTotNghiep = user.MaHocVienNavigation?.NamTotNghiep
            };

            ViewData["MaQuocTich"] = new SelectList(await _context.QuocTiches.ToListAsync(), "MaQuocTich", "TenQuocTich", model.MaQuocTich);
            return View(model);
        }

        // POST: Home/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOrHocVienOrLopTruong]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!User.Identity.IsAuthenticated) { return RedirectToAction("Login", "Account"); }
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users
                .Include(u => u.MaHocVienNavigation)
                .FirstOrDefaultAsync(u => u.MaUser == userId);
            if (user == null) { return NotFound(); }

            if (ModelState.IsValid)
            {
                try
                {
                    if (user.MaHocVienNavigation != null)
                    {
                        user.MaHocVienNavigation.HoTen = model.HoTen;
                        user.MaHocVienNavigation.NgaySinh = model.NgaySinh;
                        user.MaHocVienNavigation.Khoa = model.Khoa;
                        user.MaHocVienNavigation.Lop = model.Lop;
                        user.MaHocVienNavigation.Nganh = model.Nganh;
                        user.MaHocVienNavigation.DonVi = model.DonVi;
                        user.MaHocVienNavigation.MaQuocTich = model.MaQuocTich;
                        user.MaHocVienNavigation.NamTotNghiep = model.NamTotNghiep;

                        _context.Update(user.MaHocVienNavigation);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Profile));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật: " + ex.Message);
                }
            }

            // Reload dropdown nếu có lỗi
            ViewData["MaQuocTich"] = new SelectList(await _context.QuocTiches.ToListAsync(), "MaQuocTich", "TenQuocTich", model.MaQuocTich);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class DashboardViewModel
    {
        public int TotalHocVien { get; set; }
        public int TotalKhenThuong { get; set; }
        public int TotalPhuCap { get; set; }
        public int TotalTangLuong { get; set; }
        public int TotalQuocTich { get; set; }
        public int TotalUser { get; set; }
        public List<HocVienQuocTichStat> HocVienTheoQuocTich { get; set; } = new();
        public List<KhenThuongKyLuatStat> KhenThuongKyLuatTheoLoai { get; set; } = new();
        public List<HocVien> HocVienMoiNhat { get; set; } = new();
        public List<KhenThuongKyLuat> KhenThuongGanDay { get; set; } = new();
    }

    public class HocVienQuocTichStat
    {
        public string TenQuocTich { get; set; } = "";
        public int SoLuong { get; set; }
    }

    public class KhenThuongKyLuatStat
    {
        public string LoaiHinh { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal TongTien { get; set; }
    }
}
