using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MegaMarket.API.Services;
using MegaMarket.API.DTOs;

namespace MegaMarket.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ShiftTypesController : ControllerBase
{
    private readonly ShiftTypeService _shiftTypeService;

    public ShiftTypesController(ShiftTypeService shiftTypeService)
    {
        _shiftTypeService = shiftTypeService;
    }

    // GET: api/ShiftTypes
    [HttpGet]
    public async Task<IActionResult> GetShiftTypes()
    {
        try
        {
            var shiftTypes = await _shiftTypeService.GetAllShiftTypesAsync();
            return Ok(shiftTypes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load shift types: {ex.Message}" });
        }
    }

    // GET: api/ShiftTypes/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetShiftType(int id)
    {
        try
        {
            var shiftType = await _shiftTypeService.GetShiftTypeByIdAsync(id);

            if (shiftType == null)
            {
                return NotFound(new { message = "Shift type not found" });
            }

            return Ok(shiftType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load shift type: {ex.Message}" });
        }
    }

    // GET: api/ShiftTypes/name/Morning
    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetShiftTypeByName(string name)
    {
        try
        {
            var shiftType = await _shiftTypeService.GetShiftTypeByNameAsync(name);

            if (shiftType == null)
            {
                return NotFound(new { message = "Shift type not found" });
            }

            return Ok(shiftType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Failed to load shift type: {ex.Message}" });
        }
    }

    // POST: api/ShiftTypes
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreateShiftType([FromBody] ShiftTypeInputDto input)
    {
        try
        {
            var shiftType = await _shiftTypeService.CreateShiftTypeAsync(input);
            return CreatedAtAction(nameof(GetShiftType), new { id = shiftType.ShiftTypeId }, shiftType);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT: api/ShiftTypes/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdateShiftType(int id, [FromBody] ShiftTypeInputDto input)
    {
        try
        {
            var shiftType = await _shiftTypeService.UpdateShiftTypeAsync(id, input);
            return Ok(shiftType);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/ShiftTypes/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteShiftType(int id)
    {
        try
        {
            var result = await _shiftTypeService.DeleteShiftTypeAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Shift type not found" });
            }

            return Ok(new { message = "Shift type deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
