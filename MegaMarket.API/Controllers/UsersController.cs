using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs;
using System.Security.Claims;

namespace MegaMarket.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    // GET: api/Users
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load users: {ex.Message}" });
        }
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Access control: Admin can see all, Employee can only see themselves
            if (currentRole != "Admin" && currentUserId != id)
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load user: {ex.Message}" });
        }
    }

    // GET: api/Users/username/john
    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        try
        {
            var user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load user: {ex.Message}" });
        }
    }

    // POST: api/Users
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] UserInputDto input)
    {
        try
        {
            var user = await _userService.CreateUserAsync(input);
            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserInputDto input)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;

            // Access control: Admin can update all, Employee can only update themselves
            // Additionally, Employee cannot change their own role
            if (currentRole != "Admin")
            {
                if (currentUserId != id)
                {
                    return Forbid();
                }

                // Get current user to check if they're trying to change their role
                var currentUser = await _userService.GetUserByIdAsync(id);
                if (currentUser != null && currentUser.Role != input.Role)
                {
                    return BadRequest(new { message = "You cannot change your own role" });
                }
            }

            var user = await _userService.UpdateUserAsync(id, input);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Prevent admin from deleting themselves
            if (currentUserId == id)
            {
                return BadRequest(new { message = "You cannot delete your own account" });
            }

            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
