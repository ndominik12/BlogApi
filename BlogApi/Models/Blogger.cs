using System;

/// <summary>
/// Summary description for Blogger
/// </summary>
/// 
namespace BlogApi.Models
{
    public class Blogger
    {
        public Blogger()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Password { get; set; } = string.Empty;
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
    }
}
