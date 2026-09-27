using Microsoft.EntityFrameworkCore;
using MyHRExample.Models;

namespace MyHRExample.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Resume> Resumes { get; set; }
    }
}