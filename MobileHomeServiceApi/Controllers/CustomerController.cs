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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer,Admin")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerService"></param>
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{userId:int}")]
        public async Task<IActionResult> GetCustomerProfileByUserId([FromRoute] int userId)
        {
            var response = await _customerService.GetCustomerProfileByUserIdAsync(userId);
            return response.IsSucceeded ? Ok(response) : NotFound(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomersWithPagination([FromQuery] PaginationRequest request)
        {
            var response = await _customerService.GetCustomerProfilesWithPaginationAsync(request);
            return response.IsSucceeded ? Ok(response) : NotFound(response);
        }

        [HttpPut("{userId:int}")]
        public async Task<IActionResult> UpdateCustomerProfile([FromRoute] int userId, [FromBody] UpdateCustomerProfileRequest request)
        {
            var response = await _customerService.UpdateCustomerProfileAsync(userId, request);
            return response.IsSucceeded ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> DeleteCustomerByUserId([FromRoute] int userId)
        {
            var response = await _customerService.DeleteCustomerByUserIdAsync(userId);
            return response.IsSucceeded ? Ok(response) : BadRequest(response);
        }
    }
}
