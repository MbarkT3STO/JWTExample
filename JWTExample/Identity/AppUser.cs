using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTExample.Entities;
using Microsoft.AspNetCore.Identity;

namespace JWTExample.Identity;

public class AppUser : IdentityUser
{
    public ICollection<RefreshToken> RefreshTokens { get; set; }
}
