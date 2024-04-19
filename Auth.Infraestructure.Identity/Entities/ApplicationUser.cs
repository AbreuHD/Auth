using Microsoft.AspNetCore.Identity;

namespace Auth.Infraestructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
