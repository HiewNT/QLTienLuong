using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QLTienLuong.Models;


namespace QLTienLuong.Controllers
{
    [AdminOnly]
    public class QuocTichController : Controller
    {
        private readonly QltienLuongContext _context;

        public QuocTichController(QltienLuongContext context)
        {
            _context = context;
        }

        // GET: QuocTich
        public async Task<IActionResult> Index()
        {
            var quocTiches = await _context.QuocTiches
                .Include(q => q.HocViens)
                .ToListAsync();
            return View(quocTiches);
        }

        // GET: QuocTich/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quocTich = await _context.QuocTiches
                .Include(q => q.HocViens)
                .FirstOrDefaultAsync(m => m.MaQuocTich == id);
            if (quocTich == null)
            {
                return NotFound();
            }

            return View(quocTich);
        }

        // GET: QuocTich/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QuocTich/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaQuocTich,TenQuocTich")] QuocTich quocTich)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quocTich);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quocTich);
        }

        // GET: QuocTich/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quocTich = await _context.QuocTiches.FindAsync(id);
            if (quocTich == null)
            {
                return NotFound();
            }
            return View(quocTich);
        }

        // POST: QuocTich/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaQuocTich,TenQuocTich")] QuocTich quocTich)
        {
            if (id != quocTich.MaQuocTich)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quocTich);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuocTichExists(quocTich.MaQuocTich))
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
            return View(quocTich);
        }

        // GET: QuocTich/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quocTich = await _context.QuocTiches
                .FirstOrDefaultAsync(m => m.MaQuocTich == id);
            if (quocTich == null)
            {
                return NotFound();
            }

            return View(quocTich);
        }

        // POST: QuocTich/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var quocTich = await _context.QuocTiches.FindAsync(id);
            if (quocTich != null)
            {
                _context.QuocTiches.Remove(quocTich);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool QuocTichExists(string id)
        {
            return _context.QuocTiches.Any(e => e.MaQuocTich == id);
        }
    }
}
