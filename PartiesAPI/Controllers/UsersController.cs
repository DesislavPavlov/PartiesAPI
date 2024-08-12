using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PartiesAPI.DTO;
using PartiesAPI.Services;

namespace PartiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PartiesService _service;
        public UsersController(PartiesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetAllUsers()
        {
            try
            {
                var users = await _service.GetAllUsers();

                if (users == null)
                {
                    return NotFound("There are no users in the database currently!");
                }

                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            try
            {
                var user = await _service.GetUserById(id);

                if (user == null)
                {
                    return NotFound("User with ID of '{id}' does not exist!");
                }

                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _service.CreateUser(userDTO);

                return Ok(user);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var deletionSuccessful = await _service.DeleteUser(id);

                if (!deletionSuccessful)
                {
                    return BadRequest($"Either user with ID of '{id}' does not exist, or the user is an organizer of an event!");
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
