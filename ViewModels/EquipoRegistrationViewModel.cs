using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdeasCreativasApp.ViewModels
{
    public class EquipoRegistrationViewModel
    {
        [Required(ErrorMessage = "El nombre de equipo es obligatorio")]
        public string NombreDeEquipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("Password", ErrorMessage = "los passwords no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar al menos un integrante")]
        public List<int> IntegrantesIds { get; set; } = new List<int>();

        // Para poblar el MultiSelect en la vista
        public List<SelectListItem> AlumnosDisponibles { get; set; } = new List<SelectListItem>();
    }
}
