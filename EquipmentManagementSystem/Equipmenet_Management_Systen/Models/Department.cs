namespace Equipmenet_Management_Systen.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }
}
