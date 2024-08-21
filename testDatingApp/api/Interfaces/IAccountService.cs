using System;
using API.DTOs;

namespace API.Interfaces;

public interface IAccountService
{
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<UserDto> LoginAsync(LoginDto loginDto);
}
