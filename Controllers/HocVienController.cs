using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLTienLuong.Models;
using System.Data;
using System.IO;
using OfficeOpenXml;

namespace QLTienLuong.Controllers
{
    [AdminOrNhanVienTaiChinh]
    public class HocVienController : Controller
    {
        private readonly QltienLuongContext _context;

        public HocVienController(QltienLuongContext context)
        {
            _context = context;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        // GET: HocVien
        public async Task<IActionResult> Index()
        {
            var hocViens = await _context.HocViens
                .Include(h => h.MaQuocTichNavigation)
                .ToListAsync();
            return View(hocViens);
        }

        // GET: HocVien/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens
                .Include(h => h.MaQuocTichNavigation)
                .FirstOrDefaultAsync(m => m.MaHocVien == id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // GET: HocVien/Create
        public async Task<IActionResult> Create()
        {
            // Tự động sinh mã học viên
            var lastHocVien = await _context.HocViens
                .OrderByDescending(h => h.MaHocVien)
                .FirstOrDefaultAsync();
            
            string newMaHocVien = "HV001";
            if (lastHocVien != null && lastHocVien.MaHocVien.StartsWith("HV"))
            {
                var numberPart = lastHocVien.MaHocVien.Substring(2);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    newMaHocVien = $"HV{(lastNumber + 1):D3}";
                }
            }
            
            var hocVien = new HocVien
            {
                MaHocVien = newMaHocVien
            };
            
            ViewData["MaQuocTich"] = new SelectList(_context.QuocTiches, "MaQuocTich", "TenQuocTich");
            return View(hocVien);
        }

        // POST: HocVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHocVien,HoTen,NgaySinh,Khoa,Lop,Nganh,DonVi,MaQuocTich,NamTotNghiep")] HocVien hocVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hocVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaQuocTich"] = new SelectList(_context.QuocTiches, "MaQuocTich", "TenQuocTich", hocVien.MaQuocTich);
            return View(hocVien);
        }

        // GET: HocVien/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien == null)
            {
                return NotFound();
            }
            ViewData["MaQuocTich"] = new SelectList(_context.QuocTiches, "MaQuocTich", "TenQuocTich", hocVien.MaQuocTich);
            return View(hocVien);
        }

        // POST: HocVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHocVien,HoTen,NgaySinh,Khoa,Lop,Nganh,DonVi,MaQuocTich,NamTotNghiep")] HocVien hocVien)
        {
            if (id != hocVien.MaHocVien)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hocVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HocVienExists(hocVien.MaHocVien))
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
            ViewData["MaQuocTich"] = new SelectList(_context.QuocTiches, "MaQuocTich", "TenQuocTich", hocVien.MaQuocTich);
            return View(hocVien);
        }

        // GET: HocVien/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hocVien = await _context.HocViens
                .Include(h => h.MaQuocTichNavigation)
                .FirstOrDefaultAsync(m => m.MaHocVien == id);
            if (hocVien == null)
            {
                return NotFound();
            }

            return View(hocVien);
        }

        // POST: HocVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hocVien = await _context.HocViens.FindAsync(id);
            if (hocVien != null)
            {
                _context.HocViens.Remove(hocVien);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HocVienExists(string id)
        {
            return _context.HocViens.Any(e => e.MaHocVien == id);
        }

        // GET: HocVien/ImportExcel
        public IActionResult ImportExcel()
        {
            return View();
        }

        // POST: HocVien/ImportExcel
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn file Excel để import.";
                return RedirectToAction(nameof(ImportExcel));
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        for (int row = 2; row <= rowCount; row++) // Bỏ qua header
                        {
                            var maHocVien = worksheet.Cells[row, 1].Value?.ToString();
                            var hoTen = worksheet.Cells[row, 2].Value?.ToString();
                            var ngaySinhStr = worksheet.Cells[row, 3].Value?.ToString();
                            var khoa = worksheet.Cells[row, 4].Value?.ToString();
                            var lop = worksheet.Cells[row, 5].Value?.ToString();
                            var nganh = worksheet.Cells[row, 6].Value?.ToString();
                            var donVi = worksheet.Cells[row, 7].Value?.ToString();
                            var maQuocTich = worksheet.Cells[row, 8].Value?.ToString();
                            var namTotNghiepStr = worksheet.Cells[row, 9].Value?.ToString();

                            if (!string.IsNullOrEmpty(maHocVien) && !string.IsNullOrEmpty(hoTen))
                            {
                                // Kiểm tra xem học viên đã tồn tại chưa
                                var existingHocVien = await _context.HocViens.FindAsync(maHocVien);
                                if (existingHocVien == null)
                                {
                                    var hocVien = new HocVien
                                    {
                                        MaHocVien = maHocVien,
                                        HoTen = hoTen,
                                        Khoa = khoa,
                                        Lop = lop,
                                        Nganh = nganh,
                                        DonVi = donVi,
                                        MaQuocTich = maQuocTich
                                    };

                                    // Parse ngày sinh
                                    if (DateOnly.TryParse(ngaySinhStr, out var ngaySinh))
                                    {
                                        hocVien.NgaySinh = ngaySinh;
                                    }

                                    // Parse năm tốt nghiệp
                                    if (int.TryParse(namTotNghiepStr, out var namTotNghiep))
                                    {
                                        hocVien.NamTotNghiep = namTotNghiep;
                                    }

                                    _context.HocViens.Add(hocVien);
                                }
                            }
                        }

                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Import dữ liệu thành công!";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi import: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: HocVien/ExportExcel
        public async Task<IActionResult> ExportExcel()
        {
            try
            {
                var hocViens = await _context.HocViens
                    .Include(h => h.MaQuocTichNavigation)
                    .OrderBy(h => h.MaHocVien)
                    .ToListAsync();

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Danh sách Học viên");

                    // Header
                    worksheet.Cells[1, 1].Value = "STT";
                    worksheet.Cells[1, 2].Value = "Mã học viên";
                    worksheet.Cells[1, 3].Value = "Họ và tên";
                    worksheet.Cells[1, 4].Value = "Ngày sinh";
                    worksheet.Cells[1, 5].Value = "Khoa";
                    worksheet.Cells[1, 6].Value = "Lớp";
                    worksheet.Cells[1, 7].Value = "Ngành";
                    worksheet.Cells[1, 8].Value = "Đơn vị";
                    worksheet.Cells[1, 9].Value = "Quốc tịch";
                    worksheet.Cells[1, 10].Value = "Năm tốt nghiệp";

                    // Style header
                    using (var range = worksheet.Cells[1, 1, 1, 10])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                        range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                    }

                    // Data
                    for (int i = 0; i < hocViens.Count; i++)
                    {
                        var hocVien = hocViens[i];
                        int row = i + 2;

                        worksheet.Cells[row, 1].Value = i + 1;
                        worksheet.Cells[row, 2].Value = hocVien.MaHocVien;
                        worksheet.Cells[row, 3].Value = hocVien.HoTen;
                        worksheet.Cells[row, 4].Value = hocVien.NgaySinh?.ToString("dd/MM/yyyy");
                        worksheet.Cells[row, 5].Value = hocVien.Khoa;
                        worksheet.Cells[row, 6].Value = hocVien.Lop;
                        worksheet.Cells[row, 7].Value = hocVien.Nganh;
                        worksheet.Cells[row, 8].Value = hocVien.DonVi;
                        worksheet.Cells[row, 9].Value = hocVien.MaQuocTichNavigation?.TenQuocTich;
                        worksheet.Cells[row, 10].Value = hocVien.NamTotNghiep;

                        // Style data rows
                        using (var range = worksheet.Cells[row, 1, row, 10])
                        {
                            range.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }
                    }

                    // Auto-fit columns
                    worksheet.Cells.AutoFitColumns();

                    var content = package.GetAsByteArray();
                    var fileName = $"DanhSachHocVien_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi export: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
