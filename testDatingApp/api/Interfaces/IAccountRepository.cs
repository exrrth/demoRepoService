using System;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IAccountRepository
{
    Task AddUserAsync (AppUser user);
    Task<AppUser?> UserLoginAsync(string username);
}
