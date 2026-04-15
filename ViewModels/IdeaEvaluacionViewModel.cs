using System.ComponentModel.DataAnnotations;

namespace IdeasCreativasApp.ViewModels
{
    public class IdeaEvaluacionViewModel
    {
        public int IdeaId { get; set; }
        
        public string TextoIdea { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;

        public bool EsCreativa { get; set; }
        public bool BienPlanteada { get; set; }
        public bool EsOriginal { get; set; }
        
        [Required(ErrorMessage = "Debe elegir un estado de aprobación")]
        public string EstadoAprobacion { get; set; } = string.Empty; // "Aprobada" o "Reprobada"

        public string? Observaciones { get; set; }
    }
}
