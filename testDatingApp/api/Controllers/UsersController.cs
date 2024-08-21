using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [ApiController]
// [Route("api/[controller]")] // localhost:5001/api/users
////// public class UsersController(DataContext context) : BaseApiController

[Authorize]
public class UsersController(IUserService userService) : BaseApiController

{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        try
        {
            var users = await userService.GetAllUsersAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        // var users = await userService.GetAllUsersAsync();
        // return Ok(users); 

        // var users = await userRepository.GetMembersAsync();
        // var usersToReturn = mapper.Map<IEnumerable<MemberDto>>(users);
        // return (users);
    }

    [HttpGet("{username}")] // api/users/{username}
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        try
        {
            var user = await userService.GetUserAsync(username);

            if (user == null) return NotFound(); // GetUserByUsernameAsync could return null so it has to put "if" statement

            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        // var user = await userService.GetUserAsync(username);
        // if (user == null) return NotFound(); // GetUserByUsernameAsync could return null so it has to put "if" statement
        // return Ok(user);

        // var user = await userRepository.GetMemberAsync(username);
        // return mapper.Map<MemberDto>(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        try
        {
            var userUpdated = await userService.UpdateUserAsync(memberUpdateDto, User.GetUsername());

            if (!userUpdated) return BadRequest("Failed to update the user");

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        // var userUpdated = await userService.UpdateUserAsync(memberUpdateDto, User.GetUsername());
        // if (!userUpdated) return BadRequest("Failed to update the user");
        // return NoContent();

        //// var user = User.GetUsername();
        //// var resultUpdate = await userService.UpdateUserAsync(memberUpdateDto, user);
        //// if (!resultUpdate) return BadRequest("Failed to update the user");
        //// return NoContent();

        // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // find cliam (token) of that user then take the username (ex. lisa)
        // if (username == null) return BadRequest("No username found in token"); // was replaced in ClaimsExtension
        // var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        // if (user == null) return BadRequest("Could not find user");
        // mapper.Map(memberUpdateDto, user); // map from memberUpdateDto to user object
        // userRepository.Update(user); // not using this bcz don't want user to send "unchanged" data // it will return 204status anyway
        // if (await userRepository.SaveAllAsync()) return NoContent();
        // return BadRequest("Failed to update the user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        try
        {
            var username = User.GetUsername();
            var resultAddPhoto = await userService.AddPhotoAsync(file, username);
            return CreatedAtAction(nameof(GetUser), new { username }, resultAddPhoto);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        // var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        // if (user == null) return BadRequest("Cannot update user");
        // var result = await photoService.AddPhotoAsync(file);
        // if (result.Error != null) return BadRequest(result.Error.Message);
        // var photo = new Photo
        // {
        //     Url = result.SecureUrl.AbsoluteUri,
        //     PublicId = result.PublicId
        // };
        // if (user.Photos.Count == 0) photo.IsMain = true;
        // user.Photos.Add(photo);
        // if (await userRepository.SaveAllAsync())
        //     // return mapper.Map<PhotoDto>(photo);
        //     return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));
        // return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        try
        {
            var username = User.GetUsername();
            var resultSetMain = await userService.SetMainPhotoAsync(username, photoId);

            if (resultSetMain) return NoContent();
            return BadRequest("Problem setting main photo");

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        // var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        // if (user == null) return BadRequest("Could not find user");
        // var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        // if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");
        // var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        // if (currentMain != null) currentMain.IsMain = false;
        // photo.IsMain = true;
        // if (await userRepository.SaveAllAsync()) return NoContent();
        // return BadRequest("Problem setting main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        try
        {
            var username = User.GetUsername();
            var resultRemovePhoto = await userService.RemovePhotoAsync(username, photoId);
            if (resultRemovePhoto) return NoContent();
            return BadRequest("Problem deleting photo");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
        // var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());
        // if (user == null) return BadRequest("User not found");
        // var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        // if (photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");
        // if (photo.PublicId != null) // remove photo from cloud
        // {
        //     var result = await photoService.DeletePhotoAsync(photo.PublicId);
        //     if (result.Error != null) return BadRequest(result.Error.Message);
        // }
        // user.Photos.Remove(photo);
        // if (await userRepository.SaveAllAsync()) return Ok();
        // return BadRequest("Problem deleting photo");
    }

}
