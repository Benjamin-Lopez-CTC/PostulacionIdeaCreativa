using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasCreativasApp.Data;

namespace IdeasCreativasApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // La pagina principal tendra un listado de todas las ideas aprobadas 
            // junto con el equipo que la postulo y la fecha y hora de aprobacion.
            var ideasAprobadas = await _context.Ideas
                .Include(i => i.Equipo)
                .Where(i => i.Estado == "Aprobada")
                .OrderByDescending(i => i.FechaHoraPostulacion)
                .ToListAsync();

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View(ideasAprobadas);
        }
    }
}
