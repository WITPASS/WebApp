using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data
{
    //public class JsonIgnoreSerializationAttribute : Attribute { }

    public class LoginInfo
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserInfo
    {
        public string Token { get; set; }
        public string User { get; set; }
        public string Branch { get; set; }
    }

    public class Entity
    {
        public Guid Id { get; set; }
    }

    public class BranchEntity: Entity
    {
        [JsonIgnore]
        public virtual Branch Branch { get; set; }

        [JsonIgnore, ForeignKey("Branch")]
        public Guid? BranchId { get; set; }
    }

    public class Branch : Entity
    {
        [Required]
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }

    public class AppUser : BranchEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual IList<UserRole> UserRoles { get; set; }
    }

    public class Role : BranchEntity
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual IList<UserRole> UserRoles { get; set; }
    }

    public class UserRole : BranchEntity
    {
        public virtual AppUser User { get; set; }
        public virtual Role Role { get; set; }

        [ForeignKey("User")]
        public Guid? UserId { get; set; }
        [ForeignKey("Role")]
        public Guid? RoleId { get; set; }
    }

    public class Image : BranchEntity
    {
        [Required]
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
        public string Meta { get; set; }
        public int Version { get; set; }
        public bool Active { get; set; }
        public byte[] Data { get; set; } // TODO: should not be fetched in SQL event with odata expand operator.

        [NotMapped]
        public string Url => $"api/images/{Id}/{Version}";
    }
}
