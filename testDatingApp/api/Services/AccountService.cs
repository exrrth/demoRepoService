using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;

namespace API.Services;

public class AccountService(IAccountRepository accountRepository, ITokenService tokenService, IMapper mapper) : IAccountService
{
    public async Task<UserDto> LoginAsync(LoginDto loginDto)
    {
        var user = await accountRepository.UserLoginAsync(loginDto.Username) ?? throw new Exception("Inavalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computeHash.Length; i++)
        {
            if (computeHash[i] != user.PasswordHash[i])
            {
                throw new Exception("Invalid password");
            }
        }

        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Gender = user.Gender,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        using var hmac = new HMACSHA512(); // "using" statement is for using only in this method

        var user = mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        await accountRepository.AddUserAsync(user);

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }
}
