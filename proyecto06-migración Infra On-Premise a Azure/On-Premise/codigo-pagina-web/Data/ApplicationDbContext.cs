using Microsoft.EntityFrameworkCore;
using MiWebAPP.Models;

namespace MiWebAPP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Reserva> Reservas { get; set; }
    }
}
