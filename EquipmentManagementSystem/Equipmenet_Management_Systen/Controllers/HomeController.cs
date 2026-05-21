using Equipmenet_Management_Systen.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Equipmenet_Management_Systen.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var equipments = await _context.Equipments
                .Include(e => e.Department)
                .Include(e => e.ServiceProvider)
                .Take(10)
                .ToListAsync();

            return View(equipments);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}