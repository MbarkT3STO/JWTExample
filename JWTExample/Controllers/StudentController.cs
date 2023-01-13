using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using JWTExample.EFCore;
using JWTExample.Entities;
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
    readonly UserManager<IdentityUser> _userManager;
    readonly SignInManager<IdentityUser> _signInManager;

    public StudentController(AppDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _dbContext = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Student student)
    {
        // create user
        var user = new IdentityUser { UserName = student.Email, Email = student.Email };
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
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345")), SecurityAlgorithms.HmacSha256)
        );

        // return jwt token
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        var user = await _userManager.FindByNameAsync(model.UserName);
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "https://localhost:7183",
            audience: "https://localhost:7183",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345")), SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = DateTime.Now.AddMinutes(30),
            userName = user.UserName
        });
    }

    [Authorize]
    [HttpGet("students")]
    public IActionResult GetStudents()
    {
        var students = _dbContext.Students.ToList();
        return Ok(students);
    }
}
