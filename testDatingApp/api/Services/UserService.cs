using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class UserService(IUserRepository userRepository, IPhotoService photoService, IMapper mapper) : IUserService
{
    public async Task<IEnumerable<MemberDto>> GetAllUsersAsync()
    {
        var users = await userRepository.GetMembersAsync();
        return mapper.Map<IEnumerable<MemberDto>>(users);
    }

    public async Task<MemberDto> GetUserAsync(string username)
    {
        var user = await userRepository.GetMemberAsync(username);
        return mapper.Map<MemberDto>(user);
    }

    public async Task<bool> UpdateUserAsync(MemberUpdateDto memberUpdateDto, string username)
    {
        // var user = await userRepository.GetUserByUsernameAsync(username);
        // mapper.Map(memberUpdateDto, user);
        // if (await userRepository.SaveAllAsync()) return true;
        // return true;
        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) return false; // Return false if user is not found

        mapper.Map(memberUpdateDto, user); // Map from memberUpdateDto to user object

        return await userRepository.SaveAllAsync(); // Return the result of SaveAllAsync
    }
    public async Task<bool> SetMainPhotoAsync(string username, int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) throw new Exception("Could not find user");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) throw new Exception("Cannot use this as main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

        if (currentMain != null) currentMain.IsMain = false;
        photo.IsMain = true;

        userRepository.Update(user);

        return await userRepository.SaveAllAsync();
    }

    public async Task<bool> RemovePhotoAsync(string username, int photoId)
    {
        var user = await userRepository.GetUserByUsernameAsync(username);

        if (user == null) throw new Exception("User not found");

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null || photo.IsMain) throw new Exception("This photo cannot be deleted");

        if (photo.PublicId != null) // remove photo from cloud
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error != null) throw new Exception(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await userRepository.SaveAllAsync()) return true;

        throw new Exception("Problem deleting photo");
    }

    public async Task<PhotoDto> AddPhotoAsync(IFormFile file, string username)
    {
        var user = await userRepository.GetUserByUsernameAsync(username) ?? throw new Exception("Cannot update user");
        
        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null) throw new Exception(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);
        if (await userRepository.SaveAllAsync())
        {
            return new PhotoDto
            {
                Url = photo.Url,
                Id = photo.Id,
                IsMain = photo.IsMain
            }; // or can use mapper -> return mapper.Map<PhotoDto>(Photo);
        }

        throw new Exception("Problem adding photo");
    }
}
