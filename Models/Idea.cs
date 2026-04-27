using System;
using System.ComponentModel.DataAnnotations;

namespace IdeasCreativasApp.Models
{
    public class Idea
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El texto de la idea es obligatorio")]
        public string Texto { get; set; } = string.Empty;

        public DateTime FechaHoraPostulacion { get; set; }

        // Puede ser Pendiente, Aprobada, Rechazada
        public string Estado { get; set; } = "Pendiente";

        public int EquipoId { get; set; }
        public Equipo? Equipo { get; set; }

        // Propiedades controladas por el profesor
        public string? Observaciones { get; set; }
        public bool EsCreativa { get; set; }
        public bool BienPlanteada { get; set; }
        public bool EsOriginal { get; set; }
    }
}
