using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiWebAPP.Data;
using MiWebAPP.Models;
using MiWebAPP.Services;

namespace MiWebAPP.Controllers
{
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Reservas
        public async Task<IActionResult> Index()
        {
            var reservas = await _context.Reservas
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();

            return View(reservas);
        }

        // GET: /Reservas/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        // GET: /Reservas/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Reservas/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Reserva reserva)
        {
            Console.WriteLine(">>> Controlador Crear SE ESTÁ EJECUTANDO");

            if (!ModelState.IsValid)
                return View(reserva);

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            var servicio = new RabbitMqService();
            servicio.EnviarReserva($"Reserva creada con ID: {reserva.Id}");

            TempData["Mensaje"] = "Reserva creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Reservas/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Reserva reserva)
        {
            if (id != reserva.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(reserva);

            _context.Update(reserva);
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Reserva actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Reservas/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        // POST: /Reservas/EliminarConfirmado/5
        [HttpPost, ActionName("EliminarConfirmado")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();
            }

            TempData["Mensaje"] = "Reserva eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
