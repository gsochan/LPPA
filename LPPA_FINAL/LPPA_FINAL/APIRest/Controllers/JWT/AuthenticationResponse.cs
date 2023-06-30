using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIRest.Controllers.JWT
{
    public class AuthenticationResponse
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string JWToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
