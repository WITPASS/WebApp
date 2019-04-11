using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Entity
    {
        public Guid Id { get; set; }
    }

    public class AppUser : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Role : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UserRole : Entity
    {
        public virtual AppUser User { get; set; }
        public virtual Role Role { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }

        [ForeignKey("Role")]
        public Guid? RoleId { get; set; }
    }
}
