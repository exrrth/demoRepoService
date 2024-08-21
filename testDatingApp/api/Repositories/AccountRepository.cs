using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class AccountRepository(DataContext context) : IAccountRepository
{
    public async Task AddUserAsync(AppUser user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
    public async Task<AppUser?> UserLoginAsync(string username)
    {
        return await context.Users
            .Include(p => p.Photos) // give main photo when return UserDto // like join table
                .FirstOrDefaultAsync(x => x.UserName == username.ToLower());
    }
}
