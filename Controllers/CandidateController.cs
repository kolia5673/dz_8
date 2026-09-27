using Microsoft.AspNetCore.Mvc;
using MyHRExample.Data;
using MyHRExample.Models;
using MyHRExample.Services;

namespace MyHRExample.Controllers
{
    public class CandidateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly S3Service _s3Service;

        public CandidateController(
            ApplicationDbContext context,
            S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            string candidateName,
            string contactInfo,
            IFormFile resumeFile)
        {
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                ModelState.AddModelError(
                    "candidateName",
                    "Вкажіть ім'я кандидата.");
            }

            if (string.IsNullOrWhiteSpace(contactInfo))
            {
                ModelState.AddModelError(
                    "contactInfo",
                    "Вкажіть контактні дані.");
            }

            if (resumeFile == null || resumeFile.Length == 0)
            {
                ModelState.AddModelError(
                    "resumeFile",
                    "Завантажте PDF-файл.");
            }
            else if (!resumeFile.FileName.EndsWith(
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    "resumeFile",
                    "Резюме повинно бути у форматі PDF.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            string uniqueFileName =
                $"{Guid.NewGuid()}_{Path.GetFileName(resumeFile.FileName)}";

            string s3Key = $"resumes/{uniqueFileName}";

            using (Stream stream = resumeFile.OpenReadStream())
            {
                await _s3Service.UploadAsync(
                    stream,
                    s3Key,
                    "application/pdf");
            }

            var resume = new Resume
            {
                CandidateName = candidateName,
                ContactInfo = contactInfo,
                FileName = uniqueFileName,
                S3Key = s3Key,
                UploadedAt = DateTime.UtcNow
            };

            _context.Resumes.Add(resume);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}