using System;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces;

public interface IUserService
{
    Task<bool> SetMainPhotoAsync(string username, int photoId);
    Task<bool> RemovePhotoAsync(string username, int photoId);
    Task<PhotoDto> AddPhotoAsync(IFormFile file, string username);
    Task<IEnumerable<MemberDto>> GetAllUsersAsync(); 
    Task<MemberDto> GetUserAsync(string username);
    Task<bool> UpdateUserAsync(MemberUpdateDto memberUpdateDto, string username);
}
