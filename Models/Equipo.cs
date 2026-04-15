using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdeasCreativasApp.Models
{
    public class Equipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña del equipo es obligatoria")]
        public string Password { get; set; } = string.Empty;

        // Relaciones
        public ICollection<Alumno> Integrantes { get; set; } = new List<Alumno>();
        public ICollection<Idea> IdeasPostuladas { get; set; } = new List<Idea>();
    }
}
