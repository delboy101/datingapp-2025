using System.Threading.Tasks;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(
    UserManager<AppUser> userManager,
    IUnitOfWork uow,
    IPhotoService photoService) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users.ToListAsync();
        var userList = new List<object>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            userList.Add(new
            {
                user.Id,
                user.Email,
                Roles = roles.ToList()
            });
        }

        return Ok(userList);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{userId}")]
    public async Task<ActionResult<IList<string>>> EditRoles(string userId, [FromQuery] string roles)
    {
        if (string.IsNullOrWhiteSpace(roles)) return BadRequest("You must select at least one role");

        var selectedRoles = roles.Split(',').ToArray();

        var user = await userManager.FindByIdAsync(userId);

        if (user == null) return BadRequest("Could not retrieve user");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult<IReadOnlyList<PhotoForApprovalDto>>> GetPhotosForModeration()
    {
        var photos = await uow.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await uow.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return BadRequest("Could not find photo");

        var member = await uow.MemberRepository.GetMemberForUpdateAsync(photo.MemberId);

        if (member == null) return BadRequest("Could not find member");

        photo.IsApproved = true;

        if (member.ImageUrl == null)
        {
            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;
        }

        await uow.Complete();

        return NoContent();
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await uow.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return BadRequest("Could not find photo");

        if (photo.PublicId != null)
        {
            var response = await photoService.DeletePhotoAsync(photo.PublicId);

            if (response.Result.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
            {
                uow.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            uow.PhotoRepository.RemovePhoto(photo);
        }

        await uow.Complete();

        return NoContent();
    }
}
