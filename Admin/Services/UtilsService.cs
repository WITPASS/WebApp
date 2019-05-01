using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Admin
{
    internal class JwToken
    {
        public JwToken(JObject jwt)
        {
            UserId = new Guid(jwt.Property("unique_name").Value.ToString());
            Expiry = ToDate((int)jwt.Property("exp").Value);
            IssuedAt = ToDate((int)jwt.Property("iat").Value);
            NotBefore = ToDate((int)jwt.Property("nbf").Value);

            if (jwt.ContainsKey("role"))
            {
                var roleProp = jwt.Property("role");

                if (roleProp.Value.Type == JTokenType.Array)
                {
                    //Console.WriteLine("role array");
                    Roles = roleProp.Value.ToObject<string[]>();
                }
                else
                {
                    //Console.WriteLine("role single");
                    Roles = new List<string> { roleProp.Value.ToString() };
                }
            }
        }

        internal Guid UserId { get; private set; }
        internal DateTime Expiry { get; private set; }
        internal DateTime IssuedAt { get; private set; }
        internal DateTime NotBefore { get; private set; }
        internal IList<string> Roles { get; private set; } = new List<string>();

        DateTime ToDate(int seconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(seconds);
        }
    }

    public class UtilsService
    {
        internal JwToken ParseJwt(string token)
        {
            token = token.Split('.')[1];

            int mod4 = token.Length % 4;

            if (mod4 > 0)
            {
                token += new string('=', 4 - mod4);
            }

            byte[] data = Convert.FromBase64String(token);
            string decoded = Encoding.UTF8.GetString(data);
            var jwt = JObject.Parse(decoded);
            //Console.WriteLine(decoded);
            return new JwToken(jwt);
        }
    }
}
