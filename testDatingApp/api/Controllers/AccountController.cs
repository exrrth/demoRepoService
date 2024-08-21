using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Controllers;

public class AccountController(DataContext context, IAccountService accountService) : BaseApiController
{

    // public class AccountController : BaseApiController
    // {
    //     private readonly DataContext _context;
    //     private readonly IAccountService _accountService;


    //     public AccountController(DataContext context, IAccountService accountService)
    //     {
    //         _context = context;
    //         _accountService = accountService;
    //     }

    [HttpPost("register")] // account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) //(string username, string password) I changed to RegisterDto already
                                                                               // can add [FromBody], [FromQuery] to give ApiController a hint where to look at the data
    {
        if (await UserExists(registerDto.Username)) return BadRequest("Username is already taken");

        try
        {
            var user = await accountService.RegisterAsync(registerDto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        try
        {
            var user = await accountService.LoginAsync(loginDto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    private async Task<bool> UserExists(string username)
    {

        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }


}
