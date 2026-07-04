namespace MVC.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Specialties { get; set; } = null!;
    }
}
