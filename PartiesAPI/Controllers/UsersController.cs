using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Session;
using MySqlX.XDevAPI.Common;
using PartiesAPI.DTO;
using PartiesAPI.Exceptions;
using PartiesAPI.Services.UserService;

namespace PartiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<UserDTO>> GetAllUsers()
        {
            ActionResult result;

            try
            {
                var users = await _service.GetAllUsers();

                result = Ok(users);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            ActionResult result;

            try
            {
                var user = await _service.GetUserById(id);

                result = Ok(user);
            }
            catch (NotFoundException ex)
            {
                result = NotFound(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] UserCreateDTO userCreateDTO)
        {
            ActionResult result;

            try
            {
                var user = await _service.CreateUser(userCreateDTO);

                result = Ok(user);
            }
            catch (BadHttpRequestException ex)
            {
                result = BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                result = Conflict(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            ActionResult result;

            try
            {
                await _service.DeleteUser(id);

                result = NoContent();
            }
            catch (NotFoundException ex)
            {
                result = NotFound(ex.Message);
            }
            catch (OrganizerDeletionException ex)
            {
                result = Conflict(ex.Message);
            }
            catch (DatabaseOperationException ex)
            {
                result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return result;
        }
    }
}
