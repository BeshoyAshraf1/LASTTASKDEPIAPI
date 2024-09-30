using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lastSETIONDEPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailsController : ControllerBase
    {
        [HttpGet("anonymous")]
        [AllowAnonymous]
        public IActionResult getanaonymous()
        {
            return Ok("This is beshoy anonymous");
        }

        [HttpGet("teachers-students")]
        [Authorize(Roles = "Teacher,Student")]
        public IActionResult getBoth()
        {
            return Ok("This is beshoy teachers and students.");
        }

        [HttpGet("teachers")]
        [Authorize(Roles = "Teacher")]
        public IActionResult getteachers()
        {
            return Ok("This is beshoy teachers only.");
        }

        [HttpGet("students")]
        [Authorize(Roles = "Student")]
        public IActionResult getstudents()
        {
            return Ok("This is beshoy students only.");
        }
    }

}
