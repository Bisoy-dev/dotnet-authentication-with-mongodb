using System.Security.Claims;
using System;
using System.Runtime.InteropServices;
using System.Text;
using LearnHttpContext.Dtos;
using LearnHttpContext.Helpers;
using LearnHttpContext.Helpers.Jwt;
using LearnHttpContext.Models;
using LearnHttpContext.Services.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace LearnHttpContext.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin, Admin, User")]
public class UserController : ControllerBase
{
    
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly IJwtGenerator _jwtGenerator;
    private readonly IMongoDatabase _db;

    public UserController(
        IUserService userService,
        IRoleService roleService, 
        IJwtGenerator jwtGenerator, 
        IMongoDatabase db)
    {
        _userService = userService;
        _roleService = roleService;
        _jwtGenerator = jwtGenerator;
        _db = db;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]CreateUserDto userDto)
    {
        if(!await _userService.IsEmailUnique(userDto.Email)) return BadRequest(new { Message = "Email is already taken." });

        var user = new User();
        // hashed the password
        var client = _db.Client;
        
        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            PasswordHelper.Hash(userDto.Password, out byte[] hashed, out byte[] salt);
            user.Email = userDto.Email;
            user.Password = Convert.ToBase64String(hashed);
            user.PasswordSalt = Convert.ToBase64String(salt);

            var userCollection = _db.GetCollection<User>(UserService.USER_COLLECTION);
            var roleCollection = _db.GetCollection<Role>(RoleService.ROLE_COLLECTION);

            await userCollection.InsertOneAsync(session, user);

            var role = (await roleCollection.FindAsync(r => r.Name == userDto.Roles.FirstOrDefault())).First();

            user.Roles.Add(role.RoleId);
            await userCollection.ReplaceOneAsync(session, Builders<User>.Filter.Eq(u => u.UserId, user.UserId), user);
        
            // var result = await _userService.Create(user);
            var token = await _jwtGenerator.Generate(user.UserId, user.Email, user.Roles.ToArray());
            await session.CommitTransactionAsync();
            return Ok(new 
                { 
                    UserId = user.UserId,
                    Email = user.Email,
                    Token = token
                });
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            return BadRequest(ex.Message);
        }
        
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserDto userDto)
    {
        // Console.WriteLine(userDto.Email)
        var user = await _userService.FindByEmail(userDto.Email);
        if(user is null) return BadRequest(new { Message = "Inavlid username or email" });

        if(!PasswordHelper.Verify(userDto.Password, Convert.FromBase64String(user.Password), Convert.FromBase64String(user.PasswordSalt))) return BadRequest(new { Message = "Incorrect password" });

        return Ok(new 
            { 
                UserId = user.UserId,
                Email = user.Email,
                Token = await _jwtGenerator.Generate(user.UserId, user.Email, user.Roles.ToArray()) 
            });
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        // var claims = HttpContext.User.Claims.ToList();
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if(userId is null) return BadRequest(new { Message = "UserId is null." });

        var user = await _userService.GetUserById(userId!);
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody]CreateRole roleDto)
    {
        foreach(var role in roleDto.Names)
        {
            if(!await _roleService.IsRoleExist(role))
            {
                await _roleService.Create(new Role { Name = role });
            }
        }

        return Ok(new { Message = "Successfully added." });
    }
}