using System.ComponentModel.DataAnnotations;

namespace Equipmenet_Management_Systen.Models
{
    public class ServiceProvider
    {
        public int Id { get; set; }

        [Required]
        public string CompanyName { get; set; }

        public string ContactPerson { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public string Address { get; set; }

        public string ScopeOfWork { get; set; }

        public string SupportedMachines { get; set; }

        public string QCProductCertification { get; set; }

        public DateTime? AmcStartDate { get; set; }

        public DateTime? AmcEndDate { get; set; }

        public string ServiceType { get; set; }

        public string ApprovalStatus { get; set; } = "Pending";

        public string ApprovedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}
