using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLTienLuong.Models;


namespace QLTienLuong.Controllers
{
    [AdminOrNhanVienTaiChinh]
    public class KhenThuongKyLuatController : Controller
    {
        private readonly QltienLuongContext _context;

        public KhenThuongKyLuatController(QltienLuongContext context)
        {
            _context = context;
        }

        // GET: KhenThuongKyLuat
        public async Task<IActionResult> Index()
        {
            var khenThuongKyLuats = await _context.KhenThuongKyLuats
                .Include(k => k.MaHocVienNavigation)
                .ToListAsync();
            return View(khenThuongKyLuats);
        }

        // GET: KhenThuongKyLuat/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khenThuongKyLuat = await _context.KhenThuongKyLuats
                .Include(k => k.MaHocVienNavigation)
                .FirstOrDefaultAsync(m => m.MaQuyetDinh == id);
            if (khenThuongKyLuat == null)
            {
                return NotFound();
            }

            return View(khenThuongKyLuat);
        }

        // GET: KhenThuongKyLuat/Create
        public async Task<IActionResult> Create()
        {
            // Tự động sinh mã quyết định
            var lastKhenThuong = await _context.KhenThuongKyLuats
                .OrderByDescending(k => k.MaQuyetDinh)
                .FirstOrDefaultAsync();
            
            string newMaQuyetDinh = "QD001";
            if (lastKhenThuong != null && lastKhenThuong.MaQuyetDinh.StartsWith("QD"))
            {
                var numberPart = lastKhenThuong.MaQuyetDinh.Substring(2);
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    newMaQuyetDinh = $"QD{(lastNumber + 1):D3}";
                }
            }
            
            var khenThuong = new KhenThuongKyLuat
            {
                MaQuyetDinh = newMaQuyetDinh
            };
            
            ViewData["MaHocVien"] = new SelectList(
                _context.HocViens.Select(h => new { Value = h.MaHocVien, Text = $"{h.MaHocVien} - {h.HoTen}" }), 
                "Value", "Text"
            );
            return View(khenThuong);
        }

        // POST: KhenThuongKyLuat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaQuyetDinh,MaHocVien,LoaiHinh,NgayQuyetDinh,LyDo,SoTien")] KhenThuongKyLuat khenThuongKyLuat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khenThuongKyLuat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHocVien"] = new SelectList(
                _context.HocViens.Select(h => new { Value = h.MaHocVien, Text = $"{h.MaHocVien} - {h.HoTen}" }), 
                "Value", "Text", khenThuongKyLuat.MaHocVien
            );
            return View(khenThuongKyLuat);
        }

        // GET: KhenThuongKyLuat/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khenThuongKyLuat = await _context.KhenThuongKyLuats.FindAsync(id);
            if (khenThuongKyLuat == null)
            {
                return NotFound();
            }
            ViewData["MaHocVien"] = new SelectList(
                _context.HocViens.Select(h => new { Value = h.MaHocVien, Text = $"{h.MaHocVien} - {h.HoTen}" }), 
                "Value", "Text", khenThuongKyLuat.MaHocVien
            );
            return View(khenThuongKyLuat);
        }

        // POST: KhenThuongKyLuat/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaQuyetDinh,MaHocVien,LoaiHinh,NgayQuyetDinh,LyDo,SoTien")] KhenThuongKyLuat khenThuongKyLuat)
        {
            if (id != khenThuongKyLuat.MaQuyetDinh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khenThuongKyLuat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhenThuongKyLuatExists(khenThuongKyLuat.MaQuyetDinh))
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
            ViewData["MaHocVien"] = new SelectList(
                _context.HocViens.Select(h => new { Value = h.MaHocVien, Text = $"{h.MaHocVien} - {h.HoTen}" }), 
                "Value", "Text", khenThuongKyLuat.MaHocVien
            );
            return View(khenThuongKyLuat);
        }

        // GET: KhenThuongKyLuat/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khenThuongKyLuat = await _context.KhenThuongKyLuats
                .Include(k => k.MaHocVienNavigation)
                .FirstOrDefaultAsync(m => m.MaQuyetDinh == id);
            if (khenThuongKyLuat == null)
            {
                return NotFound();
            }

            return View(khenThuongKyLuat);
        }

        // POST: KhenThuongKyLuat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var khenThuongKyLuat = await _context.KhenThuongKyLuats.FindAsync(id);
            if (khenThuongKyLuat != null)
            {
                _context.KhenThuongKyLuats.Remove(khenThuongKyLuat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool KhenThuongKyLuatExists(string id)
        {
            return _context.KhenThuongKyLuats.Any(e => e.MaQuyetDinh == id);
        }
    }
}
