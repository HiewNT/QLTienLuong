using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLTienLuong.Models;


namespace QLTienLuong.Controllers
{
    [AdminOrNhanVienTaiChinh]
    public class DanhMucPhuCapController : Controller
    {
        private readonly QltienLuongContext _context;

        public DanhMucPhuCapController(QltienLuongContext context)
        {
            _context = context;
        }

        // GET: DanhMucPhuCap
        public async Task<IActionResult> Index()
        {
            var danhMucPhuCaps = await _context.DanhMucPhuCaps
                .Include(d => d.PhuCapHocViens)
                    .ThenInclude(p => p.MaHocVienNavigation)
                .ToListAsync();
            return View(danhMucPhuCaps);
        }

        // GET: DanhMucPhuCap/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMucPhuCap = await _context.DanhMucPhuCaps
                .Include(d => d.PhuCapHocViens)
                    .ThenInclude(p => p.MaHocVienNavigation)
                        .ThenInclude(h => h.MaQuocTichNavigation)
                .FirstOrDefaultAsync(m => m.MaPhuCap == id);
            if (danhMucPhuCap == null)
            {
                return NotFound();
            }

            return View(danhMucPhuCap);
        }

        // GET: DanhMucPhuCap/Create
        public async Task<IActionResult> Create()
        {
            // Tự động sinh mã phụ cấp
            var lastPhuCap = await _context.DanhMucPhuCaps
                .OrderByDescending(p => p.MaPhuCap)
                .FirstOrDefaultAsync();
            
            string newMaPhuCap = "PC001";
            if (lastPhuCap != null && lastPhuCap.MaPhuCap.StartsWith("PC"))
            {
                var numberPart = lastPhuCap.MaPhuCap.Substring(2);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    newMaPhuCap = $"PC{(lastNumber + 1):D3}";
                }
            }
            
            var phuCap = new DanhMucPhuCap
            {
                MaPhuCap = newMaPhuCap
            };
            
            return View(phuCap);
        }

        // POST: DanhMucPhuCap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaPhuCap,TenPhuCap,MoTa,MucPhuCapCoBan")] DanhMucPhuCap danhMucPhuCap)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhMucPhuCap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(danhMucPhuCap);
        }

        // GET: DanhMucPhuCap/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMucPhuCap = await _context.DanhMucPhuCaps.FindAsync(id);
            if (danhMucPhuCap == null)
            {
                return NotFound();
            }
            return View(danhMucPhuCap);
        }

        // POST: DanhMucPhuCap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaPhuCap,TenPhuCap,MoTa,MucPhuCapCoBan")] DanhMucPhuCap danhMucPhuCap)
        {
            if (id != danhMucPhuCap.MaPhuCap)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhMucPhuCap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhMucPhuCapExists(danhMucPhuCap.MaPhuCap))
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
            return View(danhMucPhuCap);
        }

        // GET: DanhMucPhuCap/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMucPhuCap = await _context.DanhMucPhuCaps
                .FirstOrDefaultAsync(m => m.MaPhuCap == id);
            if (danhMucPhuCap == null)
            {
                return NotFound();
            }

            return View(danhMucPhuCap);
        }

        // POST: DanhMucPhuCap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var danhMucPhuCap = await _context.DanhMucPhuCaps.FindAsync(id);
            if (danhMucPhuCap != null)
            {
                _context.DanhMucPhuCaps.Remove(danhMucPhuCap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // AJAX: Get current students for a phụ cấp
        [HttpGet]
        public async Task<IActionResult> GetCurrentStudents(string id)
        {
            try
            {
                var students = await _context.PhuCapHocViens
                    .Where(p => p.MaPhuCap == id)
                    .Include(p => p.MaHocVienNavigation)
                        .ThenInclude(h => h.MaQuocTichNavigation)
                    .Select(p => new
                    {
                        maPhuCapHocVien = p.MaPhuCapHocVien,
                        maHocVien = p.MaHocVien,
                        hoTen = p.MaHocVienNavigation != null ? p.MaHocVienNavigation.HoTen : "N/A",
                        quocTich = p.MaHocVienNavigation != null && p.MaHocVienNavigation.MaQuocTichNavigation != null 
                                 ? p.MaHocVienNavigation.MaQuocTichNavigation.TenQuocTich : "N/A",
                        ngayApDung = p.NgayApDung.HasValue ? p.NgayApDung.Value.ToString("dd/MM/yyyy") : "--",
                        isActive = p.NgayApDung.HasValue && p.NgayApDung.Value <= DateOnly.FromDateTime(DateTime.Now)
                    })
                    .ToListAsync();
                    
                Console.WriteLine($"Found {students.Count} students for phụ cấp {id}");
                return Json(students);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCurrentStudents: {ex.Message}");
                return Json(new List<object>());
            }
        }

        // AJAX: Get available students (not having this phụ cấp)
        [HttpGet]
        public async Task<IActionResult> GetAvailableStudents(string id)
        {
            try
            {
                Console.WriteLine($"Getting available students for phụ cấp: {id}");
                
                // First, get total students count
                var totalStudents = await _context.HocViens.CountAsync();
                Console.WriteLine($"Total students in database: {totalStudents}");
                
                // Get assigned student IDs for this phụ cấp
                var assignedStudentIds = await _context.PhuCapHocViens
                    .Where(p => p.MaPhuCap == id)
                    .Select(p => p.MaHocVien)
                    .ToListAsync();
                    
                Console.WriteLine($"Assigned students for {id}: {assignedStudentIds.Count}");
                
                // Get all students first to debug
                var allStudents = await _context.HocViens
                    .Select(h => new
                    {
                        maHocVien = h.MaHocVien,
                        hoTen = h.HoTen
                    })
                    .ToListAsync();
                    
                Console.WriteLine($"All students: {allStudents.Count}");
                foreach (var student in allStudents.Take(5))
                {
                    Console.WriteLine($"Student: {student.maHocVien} - {student.hoTen}");
                }
                
                // Get available students (not assigned to this phụ cấp)
                var availableStudents = allStudents
                    .Where(h => !assignedStudentIds.Contains(h.maHocVien) && !string.IsNullOrEmpty(h.hoTen))
                    .OrderBy(h => h.hoTen)
                    .ToList();
                    
                Console.WriteLine($"Available students for {id}: {availableStudents.Count}");
                return Json(availableStudents);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAvailableStudents: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Json(new List<object>());
            }
        }





        // GET: Add student to phụ cấp form
        [HttpGet]
        public async Task<IActionResult> AddStudentForm(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var phuCap = await _context.DanhMucPhuCaps.FindAsync(id);
            if (phuCap == null)
            {
                return NotFound();
            }

            // Get students who don't have this phụ cấp yet
            var assignedStudentIds = await _context.PhuCapHocViens
                .Where(p => p.MaPhuCap == id)
                .Select(p => p.MaHocVien)
                .ToListAsync();

            var availableStudents = await _context.HocViens
                .Include(h => h.MaQuocTichNavigation)
                .Where(h => !assignedStudentIds.Contains(h.MaHocVien) && !string.IsNullOrEmpty(h.HoTen))
                .OrderBy(h => h.HoTen)
                .ToListAsync();

            ViewData["MaHocVien"] = new SelectList(
                availableStudents.Select(h => new { 
                    Value = h.MaHocVien, 
                    Text = $"{h.MaHocVien} - {h.HoTen} - {h.Khoa} - {h.Lop} - {h.MaQuocTichNavigation?.TenQuocTich ?? "N/A"}" 
                }), 
                "Value", 
                "Text"
            );

            ViewBag.AvailableCount = availableStudents.Count;

            return View(phuCap);
        }

        // POST: Add multiple students to phụ cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudentForm(string MaPhuCap, string[] SelectedStudents, DateOnly NgayApDung)
        {
            if (string.IsNullOrEmpty(MaPhuCap) || SelectedStudents == null || SelectedStudents.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một học viên!";
                return RedirectToAction(nameof(Details), new { id = MaPhuCap });
            }

            try
            {
                var addedCount = 0;
                var existingCount = 0;
                var errors = new List<string>();

                foreach (var studentId in SelectedStudents)
                {
                    // Kiểm tra xem học viên đã có phụ cấp này chưa
                    var existingPhuCap = await _context.PhuCapHocViens
                        .FirstOrDefaultAsync(p => p.MaPhuCap == MaPhuCap && p.MaHocVien == studentId);

                    if (existingPhuCap == null)
                    {
                        var phuCapHocVien = new PhuCapHocVien
                        {
                            MaPhuCap = MaPhuCap,
                            MaHocVien = studentId,
                            NgayApDung = NgayApDung
                        };

                        _context.PhuCapHocViens.Add(phuCapHocVien);
                        addedCount++;

                        // Kiểm tra xem học viên đã có hướng phụ cấp trong tháng hiện tại chưa
                        var currentMonth = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
                        var existingHuongPhuCap = await _context.HuongPhuCaps
                            .FirstOrDefaultAsync(h => h.MaHocVien == studentId && h.ThangNam == currentMonth);

                        if (existingHuongPhuCap == null)
                        {
                            // Lấy tổng phụ cấp của học viên này (bao gồm phụ cấp mới vừa thêm)
                            var tongPhuCap = await _context.PhuCapHocViens
                                .Include(p => p.MaPhuCapNavigation)
                                .Where(p => p.MaHocVien == studentId)
                                .SumAsync(p => p.MaPhuCapNavigation.MucPhuCapCoBan ?? 0);

                            // Tạo hướng phụ cấp mới cho tháng hiện tại
                            var newHuongPhuCap = new HuongPhuCap
                            {
                                MaHocVien = studentId,
                                ThangNam = currentMonth,
                                TongPhuCap = tongPhuCap,
                                DoanPhi = 0,
                                LopHoc = 0,
                                TrUung = 0,
                                ConNhan = tongPhuCap,
                                Ky = null
                            };

                            _context.HuongPhuCaps.Add(newHuongPhuCap);
                        }
                    }
                    else
                    {
                        existingCount++;
                    }
                }

                await _context.SaveChangesAsync();

                if (addedCount > 0)
                {
                    var message = $"Đã thêm phụ cấp cho {addedCount} học viên thành công!";
                    if (existingCount > 0)
                    {
                        message += $" ({existingCount} học viên đã có phụ cấp này)";
                    }
                    TempData["Success"] = message;
                }
                else
                {
                    TempData["Warning"] = "Không có học viên nào được thêm mới!";
                }

                if (errors.Any())
                {
                    TempData["Errors"] = string.Join("<br/>", errors);
                }

                return RedirectToAction(nameof(Details), new { id = MaPhuCap });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi thêm: " + ex.Message);
            }

            // Reload data if validation fails
            var phuCap = await _context.DanhMucPhuCaps.FindAsync(MaPhuCap);
            var assignedStudentIds = await _context.PhuCapHocViens
                .Where(p => p.MaPhuCap == MaPhuCap)
                .Select(p => p.MaHocVien)
                .ToListAsync();

            var availableStudents = await _context.HocViens
                .Where(h => !assignedStudentIds.Contains(h.MaHocVien) && !string.IsNullOrEmpty(h.HoTen))
                .OrderBy(h => h.HoTen)
                .ToListAsync();

            ViewData["MaHocVien"] = new SelectList(
                availableStudents.Select(h => new { 
                    Value = h.MaHocVien, 
                    Text = $"{h.MaHocVien} - {h.HoTen} - {h.Khoa} - {h.Lop} - {h.MaQuocTichNavigation?.TenQuocTich ?? "N/A"}" 
                }), 
                "Value", 
                "Text"
            );

            ViewBag.AvailableCount = availableStudents.Count;

            return View(phuCap);
        }

        // POST: Remove student from phụ cấp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int phuCapHocVienId)
        {
            try
            {
                var phuCapHocVien = await _context.PhuCapHocViens
                    .Include(p => p.MaPhuCapNavigation)
                    .FirstOrDefaultAsync(p => p.MaPhuCapHocVien == phuCapHocVienId);
                
                if (phuCapHocVien == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy phụ cấp học viên!" });
                }

                var phuCapId = phuCapHocVien.MaPhuCap;
                var hocVienId = phuCapHocVien.MaHocVien;

                _context.PhuCapHocViens.Remove(phuCapHocVien);
                await _context.SaveChangesAsync();
                
                return Json(new { 
                    success = true, 
                    message = $"Đã xóa phụ cấp '{phuCapHocVien.MaPhuCapNavigation?.TenPhuCap}' khỏi học viên {hocVienId} thành công!" 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }

        private bool DanhMucPhuCapExists(string id)
        {
            return _context.DanhMucPhuCaps.Any(e => e.MaPhuCap == id);
        }






    }
}
