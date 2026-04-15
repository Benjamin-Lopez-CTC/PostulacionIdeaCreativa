using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdeasCreativasApp.ViewModels
{
    public class AddIntegranteViewModel
    {
        [Required(ErrorMessage = "El nombre de equipo es obligatorio")]
        public string NombreDeEquipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe elegir un alumno para unir")]
        public int AlumnoId { get; set; }

        public List<SelectListItem> AlumnosDisponibles { get; set; } = new List<SelectListItem>();
    }
}
