using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController: ControllerBase
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context=context;
        _tokenService=tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        AppUser user;
        try
        {
            user = await CreateUser(registerDto.username, registerDto.password);
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
        
        await _context.Users.AddAsync(user);
        _context.SaveChanges();

       var _token= _tokenService.CreateToken(user);        
       return new UserDto{
            username= user.UserName,
            token= _token        
        };       
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto){
        var user = await _context.Users.FirstOrDefaultAsync(user=>user.UserName==loginDto.username.ToLower());
        if(user==null)
            return Unauthorized("Invalid username");
        
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.password));
        for(int i=0; i<ComputeHash.Length; i++){
            if(ComputeHash[i]!=user.PasswordHash[i]){
                return Unauthorized("Invalid password");
            }
        }

        var _token=_tokenService.CreateToken(user);
        return new UserDto{
            username=user.UserName,
            token=_token
        };
    }
   
    private async Task<AppUser> CreateUser(string username, string password)
    {
        // Check if username already exists
        if(await _context.Users.AnyAsync(user=>user.UserName.ToLower()==username.ToLower()))
            throw new Exception("Username already exists");

        using var hmac = new HMACSHA512();
        return new AppUser
        {
            UserName = username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            PasswordSalt = hmac.Key
        };
    }
}
