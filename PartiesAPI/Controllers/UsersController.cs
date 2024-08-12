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
            var users = await _service.GetAllUsers();
            if (users == null) 
                return NotFound("There are no users in the database currently!");

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            var user = await _service.GetUserById(id);
            if (user == null)
                return NotFound("User with ID of '{id}' does not exist!");

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _service.CreateUser(userDTO);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var deletionSuccessful = await _service.DeleteUser(id);
            if (!deletionSuccessful)
                return BadRequest($"Either user with ID of '{id}' does not exist, or the user is an organizer of an event!");

            return NoContent();
        }
    }
}
