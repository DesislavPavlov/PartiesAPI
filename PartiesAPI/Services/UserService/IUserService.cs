using PartiesAPI.DTO;

namespace PartiesAPI.Services.UserService
{
    public interface IUserService
    {
        public Task<List<UserDTO>> GetAllUsers();
        public Task<UserDTO> GetUserById(int id);
        public Task<UserDTO> CreateUser(UserDTO userDTO);
        public Task DeleteUser(int id);
    }
}
