using Microsoft.AspNetCore.Mvc;
using MiWebAPP.Data;
using MiWebAPP.Models;
using MiWebAPP.Services;

namespace MiWebAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RabbitMqService _rabbitMqService;

        public HomeController(ApplicationDbContext context, RabbitMqService rabbitMqService)
        {
            _context = context;
            _rabbitMqService = rabbitMqService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearReserva(Reserva reserva)
        {
            if (!ModelState.IsValid)
                return View("Index");

            // 1. Guardar en BD
            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            // 2. Enviar a RabbitMQ
            _rabbitMqService.EnviarReserva(
                $"Reserva creada: {reserva.Nombre} - {reserva.Email} - {reserva.FechaReserva} - {reserva.TipoAventura}"
            );

            // 3. Mostrar popup
            ViewBag.Popup = "Reserva realizada con éxito";

            return View("Index");
        }
    }
}
