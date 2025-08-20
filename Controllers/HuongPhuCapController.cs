using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLTienLuong.Models;
using OfficeOpenXml;
using System.Data;
using System.IO;



namespace QLTienLuong.Controllers
{
    [AdminOrNhanVienTaiChinh]
    public class HuongPhuCapController : Controller
    {
        private readonly QltienLuongContext _context;

        public HuongPhuCapController(QltienLuongContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // GET: HuongPhuCap
        public async Task<IActionResult> Index(string? thangNam = null)
        {
            // Nếu không có tháng/năm được chọn, sử dụng tháng hiện tại
            if (string.IsNullOrEmpty(thangNam))
            {
                thangNam = DateTime.Now.ToString("yyyy-MM");
            }

            // Parse tháng/năm
            if (DateOnly.TryParse(thangNam + "-01", out var selectedDate))
            {
                var startOfMonth = new DateOnly(selectedDate.Year, selectedDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var huongPhuCaps = await _context.HuongPhuCaps
                    .Include(h => h.MaHocVienNavigation)
                    .Where(h => h.ThangNam >= startOfMonth && 
                               h.ThangNam <= endOfMonth)
                    .OrderBy(h => h.ThangNam)
                    .ThenBy(h => h.MaHocVien)
                    .ToListAsync();

                var viewModels = new List<HuongPhuCapViewModel>();

                foreach (var huongPhuCap in huongPhuCaps)
                {
                    // Lấy các danh mục phụ cấp được áp dụng cho học viên này
                    var phuCapHocViens = await _context.PhuCapHocViens
                        .Include(p => p.MaPhuCapNavigation)
                        .Where(p => p.MaHocVien == huongPhuCap.MaHocVien)
                        .ToListAsync();

                    var danhMucPhuCaps = phuCapHocViens.Select(p => new PhuCapDetail
                    {
                        MaPhuCap = p.MaPhuCap ?? "",
                        TenPhuCap = p.MaPhuCapNavigation?.TenPhuCap ?? "",
                        MucPhuCap = p.MaPhuCapNavigation?.MucPhuCapCoBan,
                        NgayApDung = p.NgayApDung
                    }).ToList();

                    // Tính toán các loại phụ cấp cụ thể
                    var phuCapCoBan = danhMucPhuCaps
                        .Where(p => p.TenPhuCap.Contains("NCS") || p.TenPhuCap.Contains("Đại Học"))
                        .Sum(p => p.MucPhuCap ?? 0);
                    
                    var phuCapT7CN = danhMucPhuCaps
                        .Where(p => p.TenPhuCap.Contains("T7 + CN"))
                        .Sum(p => p.MucPhuCap ?? 0);
                    
                    var phuCapSua = danhMucPhuCaps
                        .Where(p => p.TenPhuCap.Contains("Sữa") || p.TenPhuCap.Contains("sữa"))
                        .Sum(p => p.MucPhuCap ?? 0);

                    var viewModel = new HuongPhuCapViewModel
                    {
                        MaHuongPhuCap = huongPhuCap.MaHuongPhuCap,
                        MaHocVien = huongPhuCap.MaHocVien,
                        HoTen = huongPhuCap.MaHocVienNavigation?.HoTen,
                        Khoa = huongPhuCap.MaHocVienNavigation?.Khoa,
                        Lop = huongPhuCap.MaHocVienNavigation?.Lop,
                        ThangNam = huongPhuCap.ThangNam,
                        Ky = huongPhuCap.Ky,
                        DanhMucPhuCaps = danhMucPhuCaps,
                        DoanPhi = huongPhuCap.DoanPhi,
                        LopHoc = huongPhuCap.LopHoc,
                        TrUung = huongPhuCap.TrUung,
                        ConNhan = huongPhuCap.ConNhan,
                        PhuCapCoBan = phuCapCoBan,
                        PhuCapT7CN = phuCapT7CN,
                        PhuCapSua = phuCapSua
                    };

                    viewModels.Add(viewModel);
                }

                // Lấy danh sách tháng/năm có sẵn cho dropdown
                var availableMonths = await _context.HuongPhuCaps
                    .Where(h => h.ThangNam.HasValue)
                    .Select(h => h.ThangNam.Value)
                    .Distinct()
                    .OrderByDescending(d => d)
                    .Select(d => d.ToString("yyyy-MM"))
                    .ToListAsync();

                // Thêm tháng hiện tại nếu chưa có trong danh sách
                var currentMonth = DateTime.Now.ToString("yyyy-MM");
                if (!availableMonths.Contains(currentMonth))
                {
                    availableMonths.Insert(0, currentMonth);
                }

                ViewBag.ThangNamList = availableMonths;
                ViewBag.SelectedThangNam = thangNam;

                return View(viewModels);
            }

            // Nếu không parse được tháng/năm, trả về danh sách rỗng
            ViewBag.ThangNamList = new List<string>();
            ViewBag.SelectedThangNam = thangNam;
            return View(new List<HuongPhuCapViewModel>());
        }

        // GET: HuongPhuCap/GetDataByMonth
        public async Task<IActionResult> GetDataByMonth(string? thangNam = null)
        {
            // Nếu không có tháng/năm được chọn, sử dụng tháng hiện tại
            if (string.IsNullOrEmpty(thangNam))
            {
                thangNam = DateTime.Now.ToString("yyyy-MM");
            }

            // Parse tháng/năm
            if (DateOnly.TryParse(thangNam + "-01", out var selectedDate))
            {
                var startOfMonth = new DateOnly(selectedDate.Year, selectedDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var huongPhuCaps = await _context.HuongPhuCaps
                    .Include(h => h.MaHocVienNavigation)
                    .Where(h => h.ThangNam >= startOfMonth && 
                               h.ThangNam <= endOfMonth)
                    .OrderBy(h => h.ThangNam)
                    .ThenBy(h => h.MaHocVien)
                    .ToListAsync();

                var result = new List<object>();

                foreach (var huongPhuCap in huongPhuCaps)
                {
                    // Lấy các danh mục phụ cấp được áp dụng cho học viên này
                    var phuCapHocViens = await _context.PhuCapHocViens
                        .Include(p => p.MaPhuCapNavigation)
                        .Where(p => p.MaHocVien == huongPhuCap.MaHocVien)
                        .ToListAsync();

                    var danhMucPhuCaps = phuCapHocViens.Select(p => new PhuCapDetail
                    {
                        MaPhuCap = p.MaPhuCap ?? "",
                        TenPhuCap = p.MaPhuCapNavigation?.TenPhuCap ?? "",
                        MucPhuCap = p.MaPhuCapNavigation?.MucPhuCapCoBan,
                        NgayApDung = p.NgayApDung
                    }).ToList();

                    // Tính toán các loại phụ cấp cụ thể
                    var phuCapCoBan = danhMucPhuCaps
                        .Where(p => p.TenPhuCap.Contains("NCS") || p.TenPhuCap.Contains("Đại Học"))
                        .Sum(p => p.MucPhuCap ?? 0);
                    
                    var phuCapT7CN = danhMucPhuCaps
                        .Where(p => p.TenPhuCap.Contains("T7 + CN"))
                        .Sum(p => p.MucPhuCap ?? 0);
                    
                    var phuCapSua = danhMucPhuCaps
                        .Where(p => p.TenPhuCap.Contains("Sữa") || p.TenPhuCap.Contains("sữa"))
                        .Sum(p => p.MucPhuCap ?? 0);

                    var tongPhuCap = danhMucPhuCaps.Sum(p => p.MucPhuCap ?? 0);
                    var conNhan = tongPhuCap - (huongPhuCap.DoanPhi ?? 0) - (huongPhuCap.LopHoc ?? 0) - (huongPhuCap.TrUung ?? 0);

                    result.Add(new
                    {
                        maHuongPhuCap = huongPhuCap.MaHuongPhuCap,
                        maHocVien = huongPhuCap.MaHocVien,
                        hoTen = huongPhuCap.MaHocVienNavigation?.HoTen,
                        khoa = huongPhuCap.MaHocVienNavigation?.Khoa,
                        thangNam = huongPhuCap.ThangNam?.ToString("MM/yyyy"),
                        phuCapCoBan = phuCapCoBan,
                        phuCapT7CN = phuCapT7CN,
                        phuCapSua = phuCapSua,
                        tinhTongPhuCap = tongPhuCap,
                        doanPhi = huongPhuCap.DoanPhi,
                        lopHoc = huongPhuCap.LopHoc,
                        trUung = huongPhuCap.TrUung,
                        tinhConNhan = conNhan,
                        ky = huongPhuCap.Ky
                    });
                }

                return Json(result);
            }

            return Json(new List<object>());
        }

        // GET: HuongPhuCap/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huongPhuCap = await _context.HuongPhuCaps
                .Include(h => h.MaHocVienNavigation)
                .FirstOrDefaultAsync(m => m.MaHuongPhuCap == id);
            if (huongPhuCap == null)
            {
                return NotFound();
            }

            // Lấy các danh mục phụ cấp được áp dụng cho học viên này
            var phuCapHocViens = await _context.PhuCapHocViens
                .Include(p => p.MaPhuCapNavigation)
                .Where(p => p.MaHocVien == huongPhuCap.MaHocVien)
                .ToListAsync();

            var danhMucPhuCaps = phuCapHocViens.Select(p => new PhuCapDetail
            {
                MaPhuCap = p.MaPhuCap ?? "",
                TenPhuCap = p.MaPhuCapNavigation?.TenPhuCap ?? "",
                MucPhuCap = p.MaPhuCapNavigation?.MucPhuCapCoBan,
                NgayApDung = p.NgayApDung
            }).ToList();

            // Tính toán các loại phụ cấp cụ thể
            var phuCapCoBan = danhMucPhuCaps
                .Where(p => p.TenPhuCap.Contains("NCS") || p.TenPhuCap.Contains("Đại Học"))
                .Sum(p => p.MucPhuCap ?? 0);
            
            var phuCapT7CN = danhMucPhuCaps
                .Where(p => p.TenPhuCap.Contains("T7 + CN"))
                .Sum(p => p.MucPhuCap ?? 0);
            
            var phuCapSua = danhMucPhuCaps
                .Where(p => p.TenPhuCap.Contains("Sữa") || p.TenPhuCap.Contains("sữa"))
                .Sum(p => p.MucPhuCap ?? 0);

            var viewModel = new HuongPhuCapViewModel
            {
                MaHuongPhuCap = huongPhuCap.MaHuongPhuCap,
                MaHocVien = huongPhuCap.MaHocVien,
                HoTen = huongPhuCap.MaHocVienNavigation?.HoTen,
                Khoa = huongPhuCap.MaHocVienNavigation?.Khoa,
                Lop = huongPhuCap.MaHocVienNavigation?.Lop,
                ThangNam = huongPhuCap.ThangNam,
                Ky = huongPhuCap.Ky,
                DanhMucPhuCaps = danhMucPhuCaps,
                DoanPhi = huongPhuCap.DoanPhi,
                LopHoc = huongPhuCap.LopHoc,
                TrUung = huongPhuCap.TrUung,
                ConNhan = huongPhuCap.ConNhan,
                PhuCapCoBan = phuCapCoBan,
                PhuCapT7CN = phuCapT7CN,
                PhuCapSua = phuCapSua
            };

            return View(viewModel);
        }



        // GET: HuongPhuCap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huongPhuCap = await _context.HuongPhuCaps
                .Include(h => h.MaHocVienNavigation)
                .FirstOrDefaultAsync(h => h.MaHuongPhuCap == id);
            if (huongPhuCap == null)
            {
                return NotFound();
            }

            // Lấy các danh mục phụ cấp được áp dụng cho học viên này
            var phuCapHocViens = await _context.PhuCapHocViens
                .Include(p => p.MaPhuCapNavigation)
                .Where(p => p.MaHocVien == huongPhuCap.MaHocVien)
                .ToListAsync();

            var danhMucPhuCaps = phuCapHocViens.Select(p => new PhuCapDetail
            {
                MaPhuCap = p.MaPhuCap ?? "",
                TenPhuCap = p.MaPhuCapNavigation?.TenPhuCap ?? "",
                MucPhuCap = p.MaPhuCapNavigation?.MucPhuCapCoBan,
                NgayApDung = p.NgayApDung
            }).ToList();

            // Tính toán các loại phụ cấp cụ thể
            var phuCapCoBan = danhMucPhuCaps
                .Where(p => p.TenPhuCap.Contains("NCS") || p.TenPhuCap.Contains("Đại Học"))
                .Sum(p => p.MucPhuCap ?? 0);
            
            var phuCapT7CN = danhMucPhuCaps
                .Where(p => p.TenPhuCap.Contains("T7 + CN"))
                .Sum(p => p.MucPhuCap ?? 0);
            
            var phuCapSua = danhMucPhuCaps
                .Where(p => p.TenPhuCap.Contains("Sữa") || p.TenPhuCap.Contains("sữa"))
                .Sum(p => p.MucPhuCap ?? 0);

            var viewModel = new HuongPhuCapViewModel
            {
                MaHuongPhuCap = huongPhuCap.MaHuongPhuCap,
                MaHocVien = huongPhuCap.MaHocVien,
                HoTen = huongPhuCap.MaHocVienNavigation?.HoTen,
                Khoa = huongPhuCap.MaHocVienNavigation?.Khoa,
                Lop = huongPhuCap.MaHocVienNavigation?.Lop,
                ThangNam = huongPhuCap.ThangNam,
                Ky = huongPhuCap.Ky,
                DanhMucPhuCaps = danhMucPhuCaps,
                DoanPhi = huongPhuCap.DoanPhi,
                LopHoc = huongPhuCap.LopHoc,
                TrUung = huongPhuCap.TrUung,
                ConNhan = huongPhuCap.ConNhan,
                PhuCapCoBan = phuCapCoBan,
                PhuCapT7CN = phuCapT7CN,
                PhuCapSua = phuCapSua
            };

            ViewData["MaHocVien"] = new SelectList(_context.HocViens.Where(h => h.MaHocVien != "AD000"), "MaHocVien", "HoTen", huongPhuCap.MaHocVien);
            return View(viewModel);
        }

        // POST: HuongPhuCap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHuongPhuCap,MaHocVien,ThangNam,TongPhuCap,DoanPhi,LopHoc,TrUung,Ky")] HuongPhuCapViewModel viewModel)
        {
            if (id != viewModel.MaHuongPhuCap)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy entity từ database
                    var huongPhuCap = await _context.HuongPhuCaps.FindAsync(id);
                    if (huongPhuCap == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính từ ViewModel
                    huongPhuCap.MaHocVien = viewModel.MaHocVien;
                    huongPhuCap.ThangNam = viewModel.ThangNam;
                    huongPhuCap.DoanPhi = viewModel.DoanPhi;
                    huongPhuCap.LopHoc = viewModel.LopHoc;
                    huongPhuCap.TrUung = viewModel.TrUung;
                    huongPhuCap.Ky = viewModel.Ky;

                    // Lấy tổng phụ cấp từ các danh mục phụ cấp được áp dụng cho học viên này
                    var phuCapHocViens = await _context.PhuCapHocViens
                        .Include(p => p.MaPhuCapNavigation)
                        .Where(p => p.MaHocVien == huongPhuCap.MaHocVien)
                        .ToListAsync();

                    var tongPhuCap = phuCapHocViens.Sum(p => p.MaPhuCapNavigation?.MucPhuCapCoBan ?? 0);
                    
                    // Gán giá trị TongPhuCap
                    huongPhuCap.TongPhuCap = tongPhuCap;
                    
                    // Tính toán lại ConNhan: Tổng phụ cấp - Đoàn phí - Lớp học - Ứng trước
                    huongPhuCap.ConNhan = tongPhuCap - (huongPhuCap.DoanPhi ?? 0) - (huongPhuCap.LopHoc ?? 0) - (huongPhuCap.TrUung ?? 0);

                    _context.Update(huongPhuCap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HuongPhuCapExists(viewModel.MaHuongPhuCap))
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
            ViewData["MaHocVien"] = new SelectList(_context.HocViens.Where(h => h.MaHocVien != "AD000"), "MaHocVien", "HoTen", viewModel.MaHocVien);
            return View(viewModel);
        }

        // GET: HuongPhuCap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var huongPhuCap = await _context.HuongPhuCaps
                .Include(h => h.MaHocVienNavigation)
                .FirstOrDefaultAsync(m => m.MaHuongPhuCap == id);
            if (huongPhuCap == null)
            {
                return NotFound();
            }

            return View(huongPhuCap);
        }

        // POST: HuongPhuCap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var huongPhuCap = await _context.HuongPhuCaps.FindAsync(id);
            if (huongPhuCap != null)
            {
                _context.HuongPhuCaps.Remove(huongPhuCap);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // GET: HuongPhuCap/ExportExcel
        public async Task<IActionResult> ExportExcel(string? thangNam = null)
        {
            try
            {
                // Nếu không có tháng/năm được chọn, sử dụng tháng hiện tại
                if (string.IsNullOrEmpty(thangNam))
                {
                    thangNam = DateTime.Now.ToString("yyyy-MM");
                }

                // Parse tháng/năm
                if (DateOnly.TryParse(thangNam + "-01", out var selectedDate))
                {
                    var startOfMonth = new DateOnly(selectedDate.Year, selectedDate.Month, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                    var huongPhuCaps = await _context.HuongPhuCaps
                        .Include(h => h.MaHocVienNavigation)
                        .Where(h => h.ThangNam >= startOfMonth && 
                                   h.ThangNam <= endOfMonth)
                        .OrderBy(h => h.ThangNam)
                        .ThenBy(h => h.MaHocVien)
                        .ToListAsync();

                    var viewModels = new List<HuongPhuCapViewModel>();

                    foreach (var huongPhuCap in huongPhuCaps)
                    {
                        // Lấy các danh mục phụ cấp được áp dụng cho học viên này
                        var phuCapHocViens = await _context.PhuCapHocViens
                            .Include(p => p.MaPhuCapNavigation)
                            .Where(p => p.MaHocVien == huongPhuCap.MaHocVien)
                            .ToListAsync();

                        var tongPhuCap = phuCapHocViens.Sum(p => p.MaPhuCapNavigation?.MucPhuCapCoBan ?? 0);
                        var conNhan = tongPhuCap - (huongPhuCap.DoanPhi ?? 0) - (huongPhuCap.LopHoc ?? 0) - (huongPhuCap.TrUung ?? 0);

                        var viewModel = new HuongPhuCapViewModel
                        {
                            MaHuongPhuCap = huongPhuCap.MaHuongPhuCap,
                            MaHocVien = huongPhuCap.MaHocVien,
                            HoTen = huongPhuCap.MaHocVienNavigation?.HoTen,
                            ThangNam = huongPhuCap.ThangNam,
                            TongPhuCap = tongPhuCap,
                            DoanPhi = huongPhuCap.DoanPhi,
                            LopHoc = huongPhuCap.LopHoc,
                            TrUung = huongPhuCap.TrUung,
                            ConNhan = conNhan,
                            Ky = huongPhuCap.Ky
                        };

                        viewModels.Add(viewModel);
                    }

                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add($"Hưởng phụ cấp {thangNam}");

                        // Header
                        worksheet.Cells[1, 1].Value = "STT";
                        worksheet.Cells[1, 2].Value = "Mã học viên";
                        worksheet.Cells[1, 3].Value = "Họ và tên";
                        worksheet.Cells[1, 4].Value = "Tháng/Năm";
                        worksheet.Cells[1, 5].Value = "Tổng phụ cấp";
                        worksheet.Cells[1, 6].Value = "Đoàn phí";
                        worksheet.Cells[1, 7].Value = "Lớp học";
                        worksheet.Cells[1, 8].Value = "Trừ ứng";
                        worksheet.Cells[1, 9].Value = "Còn nhận";
                        worksheet.Cells[1, 10].Value = "Ký nhận";

                        // Style header
                        using (var range = worksheet.Cells[1, 1, 1, 10])
                        {
                            range.Style.Font.Bold = true;
                            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        // Data
                        for (int i = 0; i < viewModels.Count; i++)
                        {
                            var viewModel = viewModels[i];
                            int row = i + 2;

                            worksheet.Cells[row, 1].Value = i + 1;
                            worksheet.Cells[row, 2].Value = viewModel.MaHocVien;
                            worksheet.Cells[row, 3].Value = viewModel.HoTen;
                            worksheet.Cells[row, 4].Value = viewModel.ThangNam?.ToString("MM/yyyy");
                            worksheet.Cells[row, 5].Value = viewModel.TongPhuCap;
                            worksheet.Cells[row, 6].Value = viewModel.DoanPhi;
                            worksheet.Cells[row, 7].Value = viewModel.LopHoc;
                            worksheet.Cells[row, 8].Value = viewModel.TrUung;
                            worksheet.Cells[row, 9].Value = viewModel.ConNhan;
                            worksheet.Cells[row, 10].Value = viewModel.Ky;

                            // Style data rows
                            using (var range = worksheet.Cells[row, 1, row, 10])
                            {
                                range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }
                        }

                        // Auto-fit columns
                        worksheet.Cells.AutoFitColumns();

                        var content = package.GetAsByteArray();
                        var fileName = $"HuongPhuCap_{thangNam}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }

                TempData["Error"] = "Tháng/Năm không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi export: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool HuongPhuCapExists(int id)
        {
            return _context.HuongPhuCaps.Any(e => e.MaHuongPhuCap == id);
        }
    }
}

