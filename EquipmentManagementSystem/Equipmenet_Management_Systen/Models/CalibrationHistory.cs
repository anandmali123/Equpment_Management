using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Equipmenet_Management_Systen.Models
{
    public class CalibrationHistory
    {
        public int Id { get; set; }

        public int EquipmentId { get; set; }

        public DateTime CalibrationDate { get; set; }

        public DateTime NextDueDate { get; set; }

        public DateTime? NextCalibrationDate { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string PerformedBy { get; set; }

        public string CertificateFile { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; }

        [ForeignKey("PerformedBy")]
        public virtual ApplicationUser PerformedByUser { get; set; }
    }
}
