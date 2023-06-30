using APIRest.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APIRest.DAL.Domains
{
    public class UserDAO
    {
        public int Id { get; set; }

        public string Username { get; set; }
        [JsonIgnore]
        public string Salt { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        public string Email { get; set; }

        public List<Permission> Permissions { get; set; }

    }
}
