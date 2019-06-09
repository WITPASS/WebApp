using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data
{
    //public class JsonIgnoreSerializationAttribute : Attribute { }
    public class JwToken
    {
        public JwToken(string token)
        {
            var jwt = Parse(token);

            UserId = new Guid(jwt.Property("unique_name").Value.ToString());
            Expiry = ToDate((int)jwt.Property("exp").Value);
            IssuedAt = ToDate((int)jwt.Property("iat").Value);
            NotBefore = ToDate((int)jwt.Property("nbf").Value);

            if (jwt.ContainsKey("branchid"))
            {
                BranchId = new Guid(jwt.Property("branchid").Value.ToString());
            }
            else
            {
                BranchId = Guid.Empty;
            }

            if (jwt.ContainsKey("role"))
            {
                var roleProp = jwt.Property("role");

                if (roleProp.Value.Type == JTokenType.Array)
                {
                    Roles = roleProp.Value.ToObject<string[]>();
                }
                else
                {
                    Roles = new List<string> { roleProp.Value.ToString() };
                }
            }
        }

        public Guid UserId { get; private set; }
        public Guid BranchId { get; private set; }
        public DateTime Expiry { get; private set; }
        public DateTime IssuedAt { get; private set; }
        public DateTime NotBefore { get; private set; }
        public IList<string> Roles { get; private set; } = new List<string>();

        DateTime ToDate(int seconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(seconds);
        }

        JObject Parse(string token)
        {
            var body = token.Split('.')[1];

            int mod4 = body.Length % 4;

            if (mod4 > 0)
            {
                body += new string('=', 4 - mod4);
            }

            byte[] data = Convert.FromBase64String(body);
            string decoded = Encoding.UTF8.GetString(data);
            return JObject.Parse(decoded);
        }
    }

    public class LoginInfo
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }


    /// <summary>
    /// LoggedIn User
    /// </summary>
    public class UserInfo
    {
        public string Token { get; set; }
        public string UserName { get; set; }
        public string BranchName { get; set; }
    }

    public class Entity
    {
        public Guid Id { get; set; }
    }

    public class BranchEntity : Entity
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
