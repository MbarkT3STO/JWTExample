using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTExample.EFCore;
using JWTExample.Entities;
using JWTExample.Identity;
using JWTExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWTExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    readonly AppDbContext _dbContext;
    readonly UserManager<AppUser> _userManager;
    readonly SignInManager<AppUser> _signInManager;

    public StudentController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _dbContext = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Student student)
    {
        // create user
        var user = new AppUser { UserName = student.Email, Email = student.Email };
        var result = await _userManager.CreateAsync(user, "P@ssw0rd");
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        // add student to db
        _dbContext.Students.Add(student);
        await _dbContext.SaveChangesAsync();

        // generate jwt token
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, student.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7183",
            audience: "https://localhost:7183",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345")), SecurityAlgorithms.HmacSha256)
        );

        // return jwt token
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {

        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
        if (!result.Succeeded)
        {
            return BadRequest("Invalid username or password.");
        }

        var user = await _userManager.FindByNameAsync(model.UserName);

        var token = await GenerateAccessToken(user);
        var refreshToken = await GenerateRefreshToken(user);

        return Ok(new
        {
            token,
            expiration = DateTime.Now.ToLocalTime().AddMinutes(5),
            userName = user.UserName,
            refreshToken
        });
    }

    [Authorize]
    [HttpGet("students")]
    public IActionResult GetStudents()
    {
        var students = _dbContext.Students.ToList();
        return Ok(students);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);

        if (user == null)
            return BadRequest("Invalid USER");

        var refreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.UserId == user.Id && x.Token == request.RefreshToken);

        if (refreshToken == null)
            return BadRequest("Invalid REFRESH TOKEN");

        if (refreshToken.IsUsed)
            return BadRequest("Refresh token is used");

        if (refreshToken.IsInvalidated)
            return BadRequest("Refresh token is invalidated");

        if (refreshToken.ExpiresOn < DateTime.Now)
            return BadRequest("Refresh token is expired");

        refreshToken.IsUsed = true;
        _dbContext.RefreshTokens.Update(refreshToken);
        await _dbContext.SaveChangesAsync();

        var newToken = await GenerateAccessToken(user);

        return Ok(new
        {
            token = newToken,
            expiration = DateTime.Now.ToLocalTime().AddMinutes(5),
            userName = user.UserName,
            refreshToken = request.RefreshToken
        });

    }

    private async Task<string> GenerateAccessToken(AppUser user)
    {
        //Get the user role and add it to the claims
        var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        var claims = new[]{
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, userRole)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(issuer: "https://localhost:7183", audience: "https://localhost:7183", claims: claims, expires: DateTime.Now.ToLocalTime().AddMinutes(5), signingCredentials: signingCredentials);
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return token;
    }

    private async Task<string> GenerateRefreshToken(AppUser user)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            UserId = user.Id,
            CreatedOn = DateTime.Now,
            ExpiresOn = DateTime.Now.ToLocalTime().AddHours(1)
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken.Token;
    }
}

