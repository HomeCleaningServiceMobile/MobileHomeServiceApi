using MHS.Common.DTOs;
using MHS.Service.DTOs;
using MHS.Service.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MobileHomeServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Staff,Admin")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="staffService"></param>
        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStaffProfilesWithPagination([FromQuery] PaginationRequest request)
        {
            var response = await _staffService.GetAllStaffProfilesAsync(request);
            return response.IsSucceeded ? Ok(response) : NotFound(response);
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetStaffProfileByEmployeeId([FromRoute] string employeeId)
        {
            var response = await _staffService.GetStaffProfileByEmloyeeIdAsync(employeeId);
            return response.IsSucceeded ? Ok(response) : NotFound(response);
        }

        [HttpPut("{userId:int}")]
        public async Task<IActionResult> UpdateStaffProfile([FromRoute] int userId, UpdateStaffProfileRequest request)
        {
            var response = await _staffService.UpdateStaffProfileAsync(userId, request);
            return response.IsSucceeded ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{employeeId}")]
        public async Task<IActionResult> DeleteStaffByEmployeeId([FromRoute] string employeeId)
        {
            var response = await _staffService.DeleteStaffByEmployeeIdAsync(employeeId);
            return response.IsSucceeded ? Ok(response) : BadRequest(response);
        }
    }
}
