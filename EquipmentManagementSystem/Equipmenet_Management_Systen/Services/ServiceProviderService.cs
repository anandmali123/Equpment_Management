using Equipmenet_Management_Systen.Data;
using ServiceProviderModel = Equipmenet_Management_Systen.Models.ServiceProvider;
using Microsoft.EntityFrameworkCore;

namespace Equipmenet_Management_Systen.Services
{
    public interface IServiceProviderService
    {
        Task<List<ServiceProviderModel>> GetAllProvidersAsync();
        Task<ServiceProviderModel> GetProviderByIdAsync(int id);
        Task<List<ServiceProviderModel>> GetApprovedProvidersAsync();
        Task<bool> CreateProviderAsync(ServiceProviderModel provider);
        Task<bool> UpdateProviderAsync(ServiceProviderModel provider);
        Task<bool> DeleteProviderAsync(int id);
        Task<bool> ApproveProviderAsync(int id, string approvedBy);
    }

    public class ServiceProviderService : IServiceProviderService
    {
        private readonly ApplicationDbContext _context;

        public ServiceProviderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceProviderModel>> GetAllProvidersAsync()
        {
            return await _context.ServiceProviders
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ServiceProviderModel> GetProviderByIdAsync(int id)
        {
            return await _context.ServiceProviders
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task<List<ServiceProviderModel>> GetApprovedProvidersAsync()
        {
            return await _context.ServiceProviders
                .Where(sp => sp.ApprovalStatus == "Approved")
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> CreateProviderAsync(ServiceProviderModel provider)
        {
            try
            {
                _context.ServiceProviders.Add(provider);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateProviderAsync(ServiceProviderModel provider)
        {
            try
            {
                _context.ServiceProviders.Update(provider);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteProviderAsync(int id)
        {
            try
            {
                var provider = await _context.ServiceProviders.FindAsync(id);
                if (provider == null) return false;

                _context.ServiceProviders.Remove(provider);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ApproveProviderAsync(int id, string approvedBy)
        {
            try
            {
                var provider = await _context.ServiceProviders.FindAsync(id);
                if (provider == null) return false;

                provider.ApprovalStatus = "Approved";
                provider.ApprovedBy = approvedBy;
                _context.ServiceProviders.Update(provider);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
