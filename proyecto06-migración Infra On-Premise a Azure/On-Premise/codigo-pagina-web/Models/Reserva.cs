using System.ComponentModel.DataAnnotations;

namespace MiWebAPP.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime FechaReserva { get; set; }

        [Required(ErrorMessage = "Debes seleccionar una aventura")]
        public string TipoAventura { get; set; } = null!;
    }
}
