using System.Collections.Generic;
using IdeasCreativasApp.Models;

namespace IdeasCreativasApp.ViewModels
{
    public class ValidarViewModel
    {
        public List<Idea> TodasLasIdeas { get; set; } = new();
        public List<Services.ResultadoComparacion> ParesSimilares { get; set; } = new();
        public List<Idea> IdeasSinSimilitud { get; set; } = new();
    }
}
