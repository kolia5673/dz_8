using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHRExample.Data;
using MyHRExample.Services;

namespace MyHRExample.Controllers
{
    public class HRController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly S3Service _s3Service;

        public HRController(
            ApplicationDbContext context,
            S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        public async Task<IActionResult> Index()
        {
            var resumes = await _context.Resumes
                .OrderByDescending(r => r.UploadedAt)
                .ToListAsync();

            return View(resumes);
        }

        public async Task<IActionResult> Open(int id)
        {
            var resume = await _context.Resumes.FindAsync(id);

            if (resume == null)
            {
                return NotFound();
            }

            var url = await _s3Service.GetPresignedUrlAsync(resume.S3Key);

            return Redirect(url);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var resume = await _context.Resumes.FindAsync(id);

            if (resume == null)
            {
                return NotFound();
            }

            await _s3Service.DeleteAsync(resume.S3Key);

            _context.Resumes.Remove(resume);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}