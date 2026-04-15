using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasCreativasApp.Data;
using IdeasCreativasApp.Models;
using IdeasCreativasApp.ViewModels;

namespace IdeasCreativasApp.Controllers
{
    public class IdeaController : Controller
    {
        private readonly AppDbContext _context;

        public IdeaController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Postular()
        {
            // Validaciones iniciales exigidas en la historia de usuario
            if (!await _context.Alumnos.AnyAsync())
            {
                return RedirectToAction("Create", "Alumno");
            }

            if (!await _context.Equipos.AnyAsync())
            {
                return RedirectToAction("Create", "Equipo");
            }

            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Postular(IdeaPostulacionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var equipo = await _context.Equipos
                    .FirstOrDefaultAsync(e => e.Nombre == model.NombreDelEquipo && e.Password == model.PasswordDelEquipo);

                if (equipo == null)
                {
                    ModelState.AddModelError(string.Empty, "Credenciales de equipo inválidas.");
                    return View(model);
                }

                var idea = new Idea
                {
                    Texto = model.TextoDeLaIdea,
                    FechaHoraPostulacion = DateTime.Now,
                    Estado = "Pendiente",
                    EquipoId = equipo.Id,
                    EsCreativa = false,
                    BienPlanteada = false
                };

                _context.Ideas.Add(idea);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "la idea fue postulada con éxito, espere por aprobación";
                return RedirectToAction("Postular");
            }

            return View(model);
        }
    }
}
