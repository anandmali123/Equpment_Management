using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Equipmenet_Management_Systen.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        public string EquipmentName { get; set; }

        [Required]
        public string EquipmentCode { get; set; }

        public string Description { get; set; }

        public string SerialNumber { get; set; }

        public string ManufacturerName { get; set; }

        public string ManufacturerModel { get; set; }

        public string Supplier { get; set; }

        public int? DepartmentId { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? InstallationDate { get; set; }

        public DateTime? CalibrationDate { get; set; }

        public DateTime? NextCalibrationDate { get; set; }

        public string CalibrationFrequency { get; set; }

        public string ServiceType { get; set; }

        public string? AssignedUserId { get; set; }

        public int? ServiceProviderId { get; set; }

        public string Status { get; set; } = "Active";

        public string Location { get; set; }

        public string Remarks { get; set; }

        public decimal? Value { get; set; }

        public DateTime? WarrantyExpiryDate { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }

        // Navigation properties
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [ForeignKey("AssignedUserId")]
        public virtual ApplicationUser AssignedUser { get; set; }

        [ForeignKey("ServiceProviderId")]
        public virtual ServiceProvider ServiceProvider { get; set; }

        public virtual ICollection<CalibrationHistory> CalibrationHistories { get; set; } = new List<CalibrationHistory>();
        public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
}
