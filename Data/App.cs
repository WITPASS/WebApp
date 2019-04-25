using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    public class Entity
    {
        public Guid Id { get; set; }
        public virtual Branch Branch { get; set; }

        [ForeignKey("Branch")]
        public Guid? BranchId { get; set; }
    }

    public class Branch : Entity
    {
        [Required]
        public string Name { get; set; }
    }

    public class Welcome
    {
        public string Message { get; set; }
    }

    public class AppUser : Entity
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class Role : Entity
    {
        [Required]
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

    public class Image : Entity
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Meta { get; set; }
        public bool Active { get; set; }

        [JsonConverter(typeof(ByteArrayConverter))]
        public byte[] Data { get; set; }
    }
}
