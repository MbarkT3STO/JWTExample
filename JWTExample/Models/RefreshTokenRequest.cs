using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JWTExample.Models;

public class RefreshTokenRequest
{
    public string UserName { get; set; }
    public string RefreshToken { get; set; }
}
