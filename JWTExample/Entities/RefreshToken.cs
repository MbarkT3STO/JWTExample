using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTExample.Identity;
using Microsoft.AspNetCore.Identity;

namespace JWTExample.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresOn { get; set; }
    public DateTime? RevokedOn { get; set; }
    public bool IsUsed { get; set; }
    public bool IsInvalidated { get; set; }
    public string UserId { get; set; }
    public AppUser User { get; set; }

}
