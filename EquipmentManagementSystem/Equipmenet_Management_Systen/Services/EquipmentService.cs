using Equipmenet_Management_Systen.Data;
using Equipmenet_Management_Systen.Models;
using Microsoft.EntityFrameworkCore;

namespace Equipmenet_Management_Systen.Services
{
    public interface IEquipmentService
    {
        Task<List<Equipment>> GetAllEquipmentAsync();
        Task<Equipment> GetEquipmentByIdAsync(int id);
        Task<List<Equipment>> GetDueCalibrationAsync(int days = 30);
        Task<List<Equipment>> GetOverdueEquipmentAsync();
        Task<bool> CreateEquipmentAsync(Equipment equipment);
        Task<bool> UpdateEquipmentAsync(Equipment equipment);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<List<Equipment>> GetEquipmentByDepartmentAsync(int departmentId);
    }

    public class EquipmentService : IEquipmentService
    {
        private readonly ApplicationDbContext _context;

        public EquipmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Equipment>> GetAllEquipmentAsync()
        {
            return await _context.Equipments
                .Include(e => e.Department)
                .Include(e => e.ServiceProvider)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Equipment> GetEquipmentByIdAsync(int id)
        {
            return await _context.Equipments
                .Include(e => e.Department)
                .Include(e => e.ServiceProvider)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Equipment>> GetDueCalibrationAsync(int days = 30)
        {
            var dueDate = DateTime.Now.AddDays(days);
            return await _context.Equipments
                .Include(e => e.Department)
                .Include(e => e.ServiceProvider)
                .Where(e => e.NextCalibrationDate <= dueDate && e.NextCalibrationDate > DateTime.Now)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Equipment>> GetOverdueEquipmentAsync()
        {
            return await _context.Equipments
                .Include(e => e.Department)
                .Include(e => e.ServiceProvider)
                .Where(e => e.NextCalibrationDate < DateTime.Now)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CreateEquipmentAsync(Equipment equipment)
        {
            try
            {
                _context.Equipments.Add(equipment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEquipmentAsync(Equipment equipment)
        {
            try
            {
                _context.Equipments.Update(equipment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            try
            {
                var equipment = await _context.Equipments.FindAsync(id);
                if (equipment == null) return false;

                _context.Equipments.Remove(equipment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Equipment>> GetEquipmentByDepartmentAsync(int departmentId)
        {
            return await _context.Equipments
                .Include(e => e.ServiceProvider)
                .Where(e => e.DepartmentId == departmentId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
