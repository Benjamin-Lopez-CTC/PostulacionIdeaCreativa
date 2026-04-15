using System.ComponentModel.DataAnnotations;

namespace IdeasCreativasApp.Models
{
    public class Alumno
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de cédula es obligatorio")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;

        // Relación con Equipo (puede ser nulo si no tiene equipo aún)
        public int? EquipoId { get; set; }
        public Equipo? Equipo { get; set; }
    }
}
