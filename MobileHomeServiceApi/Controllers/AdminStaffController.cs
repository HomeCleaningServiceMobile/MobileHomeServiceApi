using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;

namespace MobileHomeServiceApi.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class StaffManagementController : ControllerBase
{
    private readonly IAdminStaffService _adminStaffService;
    private readonly ILogger<StaffManagementController> _logger;

    public StaffManagementController(
        IAdminStaffService adminStaffService,
        ILogger<StaffManagementController> logger)
    {
        _adminStaffService = adminStaffService;
        _logger = logger;
    }

    /// <summary>
    /// Get all staff with pagination and filtering
    /// </summary>
    /// <param name="request">Search and pagination parameters</param>
    /// <returns>Paginated list of staff</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllStaff([FromQuery] AdminStaffSearchRequest request)
    {
        try
        {
            var result = await _adminStaffService.GetAllStaffAsync(request);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff list");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving staff list"));
        }
    }

    /// <summary>
    /// Get staff by ID with detailed information
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <returns>Staff detail information</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStaffById(int id)
    {
        try
        {
            var result = await _adminStaffService.GetStaffByIdAsync(id);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff detail for ID {StaffId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving staff details"));
        }
    }

    /// <summary>
    /// Create new staff member
    /// </summary>
    /// <param name="request">Staff creation details</param>
    /// <returns>Created staff information</returns>
    [HttpPost]
    public async Task<IActionResult> CreateStaff([FromBody] AdminCreateStaffRequest request)
    {
        try
        {
            var result = await _adminStaffService.CreateStaffAsync(request);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetStaffById), new { id = result.Data?.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff for email {Email}", request.Email);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while creating staff"));
        }
    }

    /// <summary>
    /// Update existing staff member
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <param name="request">Staff update details</param>
    /// <returns>Updated staff information</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStaff(int id, [FromBody] AdminUpdateStaffRequest request)
    {
        try
        {
            var result = await _adminStaffService.UpdateStaffAsync(id, request);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff with ID {StaffId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while updating staff"));
        }
    }

    /// <summary>
    /// Change staff status (Active, Inactive, Suspended, Banned)
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <param name="request">Status change request</param>
    /// <returns>Success result</returns>
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> ChangeStaffStatus(int id, [FromBody] StaffStatusChangeRequest request)
    {
        try
        {
            var result = await _adminStaffService.ChangeStaffStatusAsync(id, request);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing staff status with ID {StaffId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while updating staff status"));
        }
    }

    /// <summary>
    /// Delete staff member (soft delete)
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStaff(int id)
    {
        try
        {
            var result = await _adminStaffService.DeleteStaffAsync(id);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff with ID {StaffId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while deleting staff"));
        }
    }

    /// <summary>
    /// Add skill to staff member
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <param name="request">Skill details</param>
    /// <returns>Created skill information</returns>
    [HttpPost("{id}/skills")]
    public async Task<IActionResult> AddStaffSkill(int id, [FromBody] StaffSkillManagementRequest request)
    {
        try
        {
            var result = await _adminStaffService.AddStaffSkillAsync(id, request);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return CreatedAtAction(nameof(GetStaffById), new { id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding skill to staff {StaffId}", id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while adding staff skill"));
        }
    }

    /// <summary>
    /// Update staff skill
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <param name="skillId">Skill ID</param>
    /// <param name="request">Updated skill details</param>
    /// <returns>Updated skill information</returns>
    [HttpPut("{id}/skills/{skillId}")]
    public async Task<IActionResult> UpdateStaffSkill(int id, int skillId, [FromBody] StaffSkillManagementRequest request)
    {
        try
        {
            var result = await _adminStaffService.UpdateStaffSkillAsync(id, skillId, request);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff skill {SkillId} for staff {StaffId}", skillId, id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while updating staff skill"));
        }
    }

    /// <summary>
    /// Remove skill from staff member
    /// </summary>
    /// <param name="id">Staff ID</param>
    /// <param name="skillId">Skill ID</param>
    /// <returns>Success result</returns>
    [HttpDelete("{id}/skills/{skillId}")]
    public async Task<IActionResult> RemoveStaffSkill(int id, int skillId)
    {
        try
        {
            var result = await _adminStaffService.RemoveStaffSkillAsync(id, skillId);
            
            if (!result.IsSucceeded)
            {
                return result.Messages.ContainsKey("NotFound") ? NotFound(result) : BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing staff skill {SkillId} for staff {StaffId}", skillId, id);
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while removing staff skill"));
        }
    }

    /// <summary>
    /// Get top performing staff
    /// </summary>
    /// <param name="count">Number of top staff to return</param>
    /// <returns>List of top performing staff</returns>
    [HttpGet("top-performers")]
    public async Task<IActionResult> GetTopPerformingStaff([FromQuery] int count = 10)
    {
        try
        {
            var result = await _adminStaffService.GetTopPerformingStaffAsync(count);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting top performing staff");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving top performing staff"));
        }
    }

    /// <summary>
    /// Get staff statistics
    /// </summary>
    /// <returns>Staff statistics summary</returns>
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStaffStatistics()
    {
        try
        {
            var result = await _adminStaffService.GetStaffStatisticsAsync();
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff statistics");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving staff statistics"));
        }
    }

    /// <summary>
    /// Get inactive staff members
    /// </summary>
    /// <param name="daysInactive">Number of days to consider as inactive</param>
    /// <returns>List of inactive staff</returns>
    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveStaff([FromQuery] int daysInactive = 30)
    {
        try
        {
            var result = await _adminStaffService.GetInactiveStaffAsync(daysInactive);
            
            if (!result.IsSucceeded)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inactive staff");
            return StatusCode(500, new AppResponse<string>()
                .SetErrorResponse("Error", "An error occurred while retrieving inactive staff"));
        }
    }
}
