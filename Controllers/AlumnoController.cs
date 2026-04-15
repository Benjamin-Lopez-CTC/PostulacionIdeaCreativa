using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdeasCreativasApp.Data;
using IdeasCreativasApp.Models;
using IdeasCreativasApp.ViewModels;

namespace IdeasCreativasApp.Controllers
{
    public class AlumnoController : Controller
    {
        private readonly AppDbContext _context;

        public AlumnoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AlumnoRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si ya existe registrado (por cedula, opcional, pero de acuerdo con la logica "el alumno no estaba registrado")
                bool existe = await _context.Alumnos.AnyAsync(a => a.Cedula == model.Cedula);
                if (existe)
                {
                    ModelState.AddModelError("Cedula", "Ya existe un alumno con esta cédula.");
                    return View(model);
                }

                var alumno = new Alumno
                {
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    Cedula = model.Cedula,
                    Password = model.Password // Sin hashing para mantenerlo simple en prototipo, o según historia de usuario
                };

                _context.Alumnos.Add(alumno);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "registrado con exito";
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}
