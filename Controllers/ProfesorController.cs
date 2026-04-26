using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasCreativasApp.Data;
using IdeasCreativasApp.ViewModels;
using IdeasCreativasApp.Services;

namespace IdeasCreativasApp.Controllers
{
    public class ProfesorController : Controller
    {
        private readonly AppDbContext _context;

        // Credenciales hardcodeadas según requerimiento
        private const string PROFESOR_USER = "admin";
        private const string PROFESOR_PASS = "admin123";

        public ProfesorController(AppDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Validar");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == PROFESOR_USER && password == PROFESOR_PASS)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Profesor")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Validar");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Validar()
        {
            var ideas = await _context.Ideas
                .Include(i => i.Equipo)
                .OrderByDescending(i => i.FechaHoraPostulacion)
                .ToListAsync();

            if (TempData["ValidarMessage"] != null)
            {
                ViewBag.Message = TempData["ValidarMessage"].ToString();
            }

            // Lower threshold to 0.10 (10% Jaccard) to easily catch natural language similarities 
            var paresSimilares = ComparadorFrases.CompararTodas(ideas, 0.10);

            var idsSimilares = paresSimilares.SelectMany(p => new[] { p.Idea1Id, p.Idea2Id }).ToHashSet();
            var ideasSinSimilitud = ideas.Where(i => !idsSimilares.Contains(i.Id)).ToList();

            var viewModel = new ValidarViewModel
            {
                TodasLasIdeas = ideas,
                ParesSimilares = paresSimilares,
                IdeasSinSimilitud = ideasSinSimilitud
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Evaluar(int id)
        {
            var idea = await _context.Ideas.Include(i => i.Equipo).FirstOrDefaultAsync(i => i.Id == id);
            if (idea == null) return NotFound();

            var model = new IdeaEvaluacionViewModel
            {
                IdeaId = idea.Id,
                TextoIdea = idea.Texto,
                NombreEquipo = idea.Equipo?.Nombre ?? "Sin Equipo",
                EsCreativa = idea.EsCreativa,
                BienPlanteada = idea.BienPlanteada,
                EsOriginal = idea.EsOriginal,
                EstadoAprobacion = idea.Estado == "Pendiente" ? "" : idea.Estado,
                Observaciones = idea.Observaciones
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Evaluar(IdeaEvaluacionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var idea = await _context.Ideas.Include(i => i.Equipo).FirstOrDefaultAsync(i => i.Id == model.IdeaId);
            if (idea == null) return NotFound();

            // Lógica: el profesor solo puede aprobar dos por equipo (Asumo por equipo de acuerdo a las Open Questions)
            if (model.EstadoAprobacion == "Aprobada" && idea.Estado != "Aprobada")
            {
                var ideasAprobadasDelEquipo = await _context.Ideas
                    .Where(i => i.EquipoId == idea.EquipoId && i.Estado == "Aprobada")
                    .CountAsync();

                if (ideasAprobadasDelEquipo >= 2)
                {
                    ModelState.AddModelError(string.Empty, $"Ya se han aprobado 2 ideas para el equipo {idea.Equipo?.Nombre}. No se pueden aprobar más.");
                    return View(model);
                }
            }

            idea.EsCreativa = model.EsCreativa;
            idea.BienPlanteada = model.BienPlanteada;
            idea.EsOriginal = model.EsOriginal;
            idea.Observaciones = model.Observaciones;
            idea.Estado = model.EstadoAprobacion; // "Aprobada" o "Reprobada"

            await _context.SaveChangesAsync();

            // Mensaje que dicta el AC
            if (model.EstadoAprobacion == "Aprobada")
            {
                TempData["SuccessMessage"] = "idea aprobada";
            }
            else
            {
                TempData["SuccessMessage"] = "idea reprobada";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
