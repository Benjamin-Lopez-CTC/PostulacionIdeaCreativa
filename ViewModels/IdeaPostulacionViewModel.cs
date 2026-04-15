using System.ComponentModel.DataAnnotations;

namespace IdeasCreativasApp.ViewModels
{
    public class IdeaPostulacionViewModel
    {
        [Required(ErrorMessage = "El nombre del equipo es obligatorio")]
        public string NombreDelEquipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña del equipo es obligatoria")]
        public string PasswordDelEquipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El texto de la idea es obligatorio")]
        [MinLength(10, ErrorMessage = "La idea debe tener al menos 10 caracteres")]
        public string TextoDeLaIdea { get; set; } = string.Empty;
    }
}
