using Microsoft.AspNetCore.Identity;

namespace lastSETIONDEPI.Models.Data
{
    public class SchoolUser : IdentityUser
    {
        public string SchoolName { get; set; }
        public int PerformanceRate { get; set; }
    }
}
