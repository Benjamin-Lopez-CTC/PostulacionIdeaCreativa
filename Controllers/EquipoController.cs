using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IdeasCreativasApp.Data;
using IdeasCreativasApp.Models;
using IdeasCreativasApp.ViewModels;

namespace IdeasCreativasApp.Controllers
{
    public class EquipoController : Controller
    {
        private readonly AppDbContext _context;

        public EquipoController(AppDbContext context)
        {
            _context = context;
        }

        private async Task PopulaAlumnosDisponibles(EquipoRegistrationViewModel model)
        {
            var disponibles = await _context.Alumnos
                .Where(a => a.EquipoId == null)
                .ToListAsync();

            model.AlumnosDisponibles = disponibles.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Nombre} {a.Apellido} ({a.Cedula})"
            }).ToList();
        }

        public async Task<IActionResult> Create()
        {
            var model = new EquipoRegistrationViewModel();
            await PopulaAlumnosDisponibles(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EquipoRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validación 1: integranes límite
                if (model.IntegrantesIds.Count > 2)
                {
                    ModelState.AddModelError("IntegrantesIds", "el numero de integrantes no esta permitido");
                }
                
                // Validación 2: nombre único
                bool nombreOcupado = await _context.Equipos.AnyAsync(e => e.Nombre == model.NombreDeEquipo);
                if (nombreOcupado)
                {
                    ModelState.AddModelError("NombreDeEquipo", "ya hay un equipo con ese nombre, ingrese otro nombre");
                }

                // Si hay errores de negocio, recargamos la vista
                if (!ModelState.IsValid)
                {
                    await PopulaAlumnosDisponibles(model);
                    return View(model);
                }

                // Validación 3: Estar seguros de que los alumnos siguen libres
                var alumnosSeleccionados = await _context.Alumnos
                    .Where(a => model.IntegrantesIds.Contains(a.Id))
                    .ToListAsync();
                
                var alumnosOcupados = alumnosSeleccionados.Where(a => a.EquipoId != null).ToList();
                if (alumnosOcupados.Any())
                {
                    if (alumnosOcupados.Count == 1)
                    {
                         ModelState.AddModelError("IntegrantesIds", $"el/la alumno/a {alumnosOcupados.First().Nombre} ya esta en un equipo");
                    }
                    else
                    {
                         ModelState.AddModelError("IntegrantesIds", $"los alumnos {alumnosOcupados[0].Nombre} y {alumnosOcupados[1].Nombre} ya estan en un equipo");
                    }
                    await PopulaAlumnosDisponibles(model);
                    return View(model);
                }

                // OK - Proceder con registro
                var equipo = new Equipo
                {
                    Nombre = model.NombreDeEquipo,
                    Password = model.Password
                };

                // Asignar los alumnos al equipo
                equipo.Integrantes = alumnosSeleccionados;

                _context.Equipos.Add(equipo);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ingresado con exito";
                return RedirectToAction("Index", "Home");
            }

            // Si falla validacion base de inputs vacíos
            await PopulaAlumnosDisponibles(model);
            return View(model);
        }

        private async Task PopulaAlumnosDisponiblesSingle(AddIntegranteViewModel model)
        {
            var disponibles = await _context.Alumnos
                .Where(a => a.EquipoId == null)
                .ToListAsync();

            model.AlumnosDisponibles = disponibles.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.Nombre} {a.Apellido} ({a.Cedula})"
            }).ToList();
        }

        public async Task<IActionResult> AddIntegrante()
        {
            var model = new AddIntegranteViewModel();
            await PopulaAlumnosDisponiblesSingle(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddIntegrante(AddIntegranteViewModel model)
        {
            if (ModelState.IsValid)
            {
                var equipo = await _context.Equipos.Include(e => e.Integrantes)
                    .FirstOrDefaultAsync(e => e.Nombre == model.NombreDeEquipo && e.Password == model.Password);

                if (equipo == null)
                {
                    ModelState.AddModelError(string.Empty, "El nombre de equipo o la contraseña no son correctos.");
                    await PopulaAlumnosDisponiblesSingle(model);
                    return View(model);
                }

                if (equipo.Integrantes.Count >= 2)
                {
                    ModelState.AddModelError(string.Empty, "Este equipo ya tiene el máximo de 2 integrantes permitido.");
                    await PopulaAlumnosDisponiblesSingle(model);
                    return View(model);
                }

                var alumno = await _context.Alumnos.FirstOrDefaultAsync(a => a.Id == model.AlumnoId);
                if (alumno == null || alumno.EquipoId != null)
                {
                    ModelState.AddModelError("AlumnoId", "El alumno seleccionado ya no está disponible.");
                    await PopulaAlumnosDisponiblesSingle(model);
                    return View(model);
                }

                alumno.EquipoId = equipo.Id;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Integrante añadido con éxito";
                return RedirectToAction("Index", "Home");
            }

            await PopulaAlumnosDisponiblesSingle(model);
            return View(model);
        }
    }
}

