using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IdeasCreativasApp.Models;

namespace IdeasCreativasApp.Services
{
    /// <summary>
    /// Relación de una palabra con su frecuencia de aparición.
    /// </summary>
    public class PalabraComun
    {
        public string Palabra { get; set; } = string.Empty;
        public int Frecuencia { get; set; }
    }

    /// <summary>
    /// Resultado de la comparación entre dos frases.
    /// </summary>
    public class ResultadoComparacion
    {
        public int Idea1Id { get; set; }
        public int Idea2Id { get; set; }
        
        public Idea? Idea1 { get; set; }
        public Idea? Idea2 { get; set; }

        public string Frase1 { get; set; } = string.Empty;
        public string Frase2 { get; set; } = string.Empty;
        public string NombreEquipo1 { get; set; } = string.Empty;
        public string NombreEquipo2 { get; set; } = string.Empty;
        public double Similitud { get; set; }
        public List<PalabraComun> PalabrasComunes { get; set; } = new();
    }

    /// <summary>
    /// Servicio que implementa el algoritmo comparativo de frases similares
    /// basado en lematización y filtrado de stopwords en español.
    /// </summary>
    public static class ComparadorFrases
    {
        // Stopwords en español (sin acentos, agrupados de forma limpia)
        private static readonly HashSet<string> PalabrasConectoras = new(StringComparer.OrdinalIgnoreCase)
        {
            "el", "la", "los", "las", "un", "una", "unos", "unas",
            "y", "o", "u", "e", "pero", "como", "que", "de", "del", "a", "al", "en",
            "por", "para", "con", "sin", "sobre", "entre", "hasta", "desde",
            "mi", "mis", "tu", "tus", "su", "sus", "nuestro", "nuestra", "se", "lo",
            "me", "te", "le", "les", "nos", "os", "yo", "tu", "el", "ella", "ellos", "ellas",
            "ser", "es", "son", "era", "eran", "fui", "fueron", "tiene", "tienen", "hay",
            "este", "esta", "estos", "estas", "ese", "esa", "esos", "esas", "aquel", "aquella",
            "mas", "muy", "mucho", "poco", "ya", "si", "no", "cual", "sean", "de", "que" 
        };

        /// <summary>
        /// Compara todas las ideas entre sí y devuelve pares de ideas
        /// que superan el umbral de similitud.
        /// </summary>
        public static List<ResultadoComparacion> CompararTodas(List<Idea> ideas, double umbralSimilitud = 0.25)
        {
            List<ResultadoComparacion> resultados = new();

            for (int i = 0; i < ideas.Count; i++)
            {
                for (int j = i + 1; j < ideas.Count; j++)
                {
                    var (similitud, palabrasComunes) = CalcularSimilitud(ideas[i].Texto, ideas[j].Texto);

                    if (similitud >= umbralSimilitud && palabrasComunes.Count > 0)
                    {
                        resultados.Add(new ResultadoComparacion
                        {
                            Idea1Id = ideas[i].Id,
                            Idea2Id = ideas[j].Id,
                            Idea1 = ideas[i],
                            Idea2 = ideas[j],
                            Frase1 = ideas[i].Texto,
                            Frase2 = ideas[j].Texto,
                            NombreEquipo1 = ideas[i].Equipo?.Nombre ?? "Sin Equipo",
                            NombreEquipo2 = ideas[j].Equipo?.Nombre ?? "Sin Equipo",
                            Similitud = similitud,
                            PalabrasComunes = palabrasComunes
                        });
                    }
                }
            }

            // Ordenamos primero por similitud para destacar los más idénticos
            return resultados.OrderByDescending(r => r.Similitud).ToList();
        }

        /// <summary>
        /// Calcula la similitud entre dos frases.
        /// </summary>
        public static (double Similitud, List<PalabraComun> PalabrasComunes) CalcularSimilitud(string frase1, string frase2)
        {
            var (unicas1, todas1) = ObtenerPalabras(frase1);
            var (unicas2, todas2) = ObtenerPalabras(frase2);

            if (unicas1.Count == 0 && unicas2.Count == 0) 
                return (0, new List<PalabraComun>());

            int interseccion = unicas1.Intersect(unicas2, StringComparer.OrdinalIgnoreCase).Count();
            int union = unicas1.Union(unicas2, StringComparer.OrdinalIgnoreCase).Count();
            double similitud = (double)interseccion / union;

            var palabrasEnComun = unicas1.Intersect(unicas2, StringComparer.OrdinalIgnoreCase).ToList();
            var informacionPalabras = new List<PalabraComun>();

            foreach(var palabra in palabrasEnComun)
            {
                int totalApariciones = todas1.Count(p => p.Equals(palabra, StringComparison.OrdinalIgnoreCase)) +
                                       todas2.Count(p => p.Equals(palabra, StringComparison.OrdinalIgnoreCase));
                
                informacionPalabras.Add(new PalabraComun { Palabra = palabra, Frecuencia = totalApariciones });
            }

            informacionPalabras = informacionPalabras.OrderByDescending(p => p.Frecuencia).ToList();
            return (similitud, informacionPalabras);
        }

        /// <summary>
        /// Extrae las palabras significativas de una frase, removiendo stopwords
        /// y aplicando lematización, retornando unicas y todas.
        /// </summary>
        public static (HashSet<string> Unicas, List<string> Todas) ObtenerPalabras(string frase)
        {
            var unicas = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var todas = new List<string>();
            var palabraActual = new StringBuilder();

            int inicio = 0;
            if (frase.Length > 2 && char.IsDigit(frase[0]) && frase[1] == '-')
            {
                inicio = 2; // Soporte para formato consola si hubiera
            }

            string textoProcesar = frase + " "; 
            for (int i = inicio; i < textoProcesar.Length; i++)
            {
                char c = textoProcesar[i];
                if (char.IsLetterOrDigit(c))
                {
                    palabraActual.Append(c);
                }
                else if (palabraActual.Length > 0)
                {
                    string palabraOriginal = palabraActual.ToString();
                    string palabraLimpia = RemoverTildes(palabraOriginal).ToLowerInvariant();
                    
                    if (!PalabrasConectoras.Contains(palabraLimpia) && palabraLimpia.Length > 1)
                    {
                        // Extraemos la raiz (lemma/stem) de la palabra limpia
                        string lemma = Lematizar(palabraLimpia);
                        unicas.Add(lemma);
                        todas.Add(lemma);
                    }
                    palabraActual.Clear();
                }
            }

            return (unicas, todas);
        }

        /// <summary>
        /// Remueve tildes y caracteres especiales del español.
        /// </summary>
        public static string RemoverTildes(string texto)
        {
            string consignos = "áàäéèëíìïóòöúùüñÁÀÄÉÈËÍÌÏÓÒÖÚÙÜÑ";
            string sinsignos = "aaaeeeiiiooouuunAAAEEEIIIOOOUUUN";
            for (int v = 0; v < consignos.Length; v++)
            {
                texto = texto.Replace(consignos[v], sinsignos[v]);
            }
            return texto;
        }

        /// <summary>
        /// Función para Lematizar / Aplicar Stemming sencillo.
        /// </summary>
        public static string Lematizar(string palabra)
        {
            if (palabra.Length < 4) return palabra;

            string[] terminaciones = { 
                "mente", "ciones", "cion", "adas", "ados", "idas", "idos", 
                "ando", "iendo", "aron", "ieron", "amos", "emos", "imos", 
                "ado", "ida", "ido", "tas", "tos", "as", "os", "es", "ar", "er", "ir", 
                "a", "o", "e", "s" 
            };

            foreach (var term in terminaciones)
            {
                if (palabra.EndsWith(term) && palabra.Length - term.Length >= 3)
                {
                    return palabra.Substring(0, palabra.Length - term.Length);
                }
            }

            return palabra;
        }
    }
}
