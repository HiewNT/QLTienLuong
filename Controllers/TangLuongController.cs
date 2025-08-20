using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLTienLuong.Models;


namespace QLTienLuong.Controllers
{
    [AdminOrNhanVienTaiChinh]
    public class TangLuongController : Controller
    {
        private readonly QltienLuongContext _context;

        public TangLuongController(QltienLuongContext context)
        {
            _context = context;
        }

        // GET: TangLuong
        public async Task<IActionResult> Index()
        {
            var tangLuongs = await _context.TangLuongs
                .Include(t => t.HocVienTangLuongs)
                .ThenInclude(h => h.MaHocVienNavigation)
                .ThenInclude(h => h.MaQuocTichNavigation)
                .ToListAsync();
            return View(tangLuongs);
        }

        // GET: TangLuong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tangLuong = await _context.TangLuongs
                .Include(t => t.HocVienTangLuongs)
                .ThenInclude(h => h.MaHocVienNavigation)
                .ThenInclude(h => h.MaQuocTichNavigation)
                .FirstOrDefaultAsync(m => m.MaTangLuong == id);
            if (tangLuong == null)
            {
                return NotFound();
            }

            return View(tangLuong);
        }

        // GET: TangLuong/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TangLuong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MucLuongTang,NgayTangLuong")] TangLuong tangLuong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tangLuong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tangLuong);
        }

        // GET: TangLuong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tangLuong = await _context.TangLuongs.FindAsync(id);
            if (tangLuong == null)
            {
                return NotFound();
            }
            return View(tangLuong);
        }

        // POST: TangLuong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTangLuong,MucLuongTang,NgayTangLuong")] TangLuong tangLuong)
        {
            if (id != tangLuong.MaTangLuong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tangLuong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TangLuongExists(tangLuong.MaTangLuong))
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
            return View(tangLuong);
        }

        // GET: TangLuong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tangLuong = await _context.TangLuongs
                .FirstOrDefaultAsync(m => m.MaTangLuong == id);
            if (tangLuong == null)
            {
                return NotFound();
            }

            return View(tangLuong);
        }

        // POST: TangLuong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tangLuong = await _context.TangLuongs.FindAsync(id);
            if (tangLuong != null)
            {
                _context.TangLuongs.Remove(tangLuong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: TangLuong/AddStudentForm/5
        public async Task<IActionResult> AddStudentForm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tangLuong = await _context.TangLuongs
                .FirstOrDefaultAsync(t => t.MaTangLuong == id);
            if (tangLuong == null)
            {
                return NotFound();
            }

            // Lấy danh sách học viên chưa được áp dụng tăng cường này
            var appliedStudentIds = await _context.HocVienTangLuongs
                .Where(h => h.MaTangLuong == id)
                .Select(h => h.MaHocVien)
                .ToListAsync();

            var availableStudents = await _context.HocViens
                .Where(h => !appliedStudentIds.Contains(h.MaHocVien) && h.MaHocVien != "AD000")
                .Include(h => h.MaQuocTichNavigation)
                .ToListAsync();

            ViewData["MaHocVien"] = new SelectList(
                availableStudents.Select(h => new { Value = h.MaHocVien, Text = $"{h.MaHocVien} - {h.HoTen} - {h.Khoa} - {h.Lop} - {h.MaQuocTichNavigation?.TenQuocTich ?? "N/A"}" }), 
                "Value", "Text"
            );

            return View(tangLuong);
        }

        // POST: TangLuong/AddStudentForm/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudentForm(int id, string[] SelectedStudents, DateOnly? ThangNam, string GhiChu)
        {
            var tangLuong = await _context.TangLuongs
                .FirstOrDefaultAsync(t => t.MaTangLuong == id);
            if (tangLuong == null)
            {
                return NotFound();
            }

            if (SelectedStudents == null || SelectedStudents.Length == 0)
            {
                ModelState.AddModelError("", "Vui lòng chọn ít nhất một học viên!");
            }
            else
            {
                try
                {
                    var addedCount = 0;
                    var existingCount = 0;
                    var errors = new List<string>();

                    foreach (var studentId in SelectedStudents)
                    {
                        // Kiểm tra xem học viên đã được áp dụng tăng cường này chưa
                        var existing = await _context.HocVienTangLuongs
                            .FirstOrDefaultAsync(h => h.MaTangLuong == id && h.MaHocVien == studentId);
                        
                        if (existing != null)
                        {
                            existingCount++;
    
                        }
                        else
                        {
                            var hocVienTangLuong = new HocVienTangLuong
                            {
                                MaTangLuong = id,
                                MaHocVien = studentId,
                                ThangNam = ThangNam,
                                GhiChu = GhiChu
                            };
                            
                            _context.Add(hocVienTangLuong);
                            addedCount++;
                        }
                    }

                    await _context.SaveChangesAsync();

                    if (addedCount > 0)
                    {


                    }
                    else
                    {
                        TempData["Warning"] = "Không có học viên nào được thêm mới!";
                    }

                    if (errors.Any())
                    {
                        TempData["Errors"] = string.Join("<br/>", errors);
                    }

                    return RedirectToAction(nameof(Details), new { id = id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi thêm: " + ex.Message);
                }
            }

            // Reload ViewData nếu có lỗi
            var appliedStudentIds = await _context.HocVienTangLuongs
                .Where(h => h.MaTangLuong == id)
                .Select(h => h.MaHocVien)
                .ToListAsync();

            var availableStudents = await _context.HocViens
                .Where(h => !appliedStudentIds.Contains(h.MaHocVien) && h.MaHocVien != "AD000")
                .Include(h => h.MaQuocTichNavigation)
                .ToListAsync();

            ViewData["MaHocVien"] = new SelectList(
                availableStudents.Select(h => new { Value = h.MaHocVien, Text = $"{h.MaHocVien} - {h.HoTen} - {h.Khoa} - {h.Lop} - {h.MaQuocTichNavigation?.TenQuocTich ?? "N/A"}" }), 
                "Value", "Text"
            );

            return View(tangLuong);
        }

        // POST: TangLuong/RemoveStudent/5
        [HttpPost]
        public async Task<IActionResult> RemoveStudent(int id, string maHocVien)
        {
            var hocVienTangLuong = await _context.HocVienTangLuongs
                .FirstOrDefaultAsync(h => h.MaTangLuong == id && h.MaHocVien == maHocVien);
            
            if (hocVienTangLuong == null)
            {
                return NotFound();
            }

            _context.HocVienTangLuongs.Remove(hocVienTangLuong);
            await _context.SaveChangesAsync();
            
            return Json(new { success = true });
        }

        private bool TangLuongExists(int id)
        {
            return _context.TangLuongs.Any(e => e.MaTangLuong == id);
        }
    }
}
