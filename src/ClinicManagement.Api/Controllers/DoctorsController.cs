using ClinicManagement.Application.DTOs.Doctors;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagement.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpPost("signup")]
        [ProducesResponseType(typeof(DoctorSignupResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(DoctorSignupResponseDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Signup([FromBody] DoctorSignupRequestDto request)
        {
            var result = await _doctorService.SignupAsync(request);

            if (!result.IsSuccess)
            {
                return Conflict(result);
            }

            return Ok(result);
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DoctorGetResponseDto>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _doctorService.GetAllDoctorsAsync();

            return Ok(result);
        }
    }
}
