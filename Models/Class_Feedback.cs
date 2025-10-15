using System.Security.Cryptography.X509Certificates;

namespace Taste_Hub.Models
{
    public class Class_Feedback
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Message { get; set; }
    }
}
