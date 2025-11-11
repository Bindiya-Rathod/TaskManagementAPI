using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.Interfaces;
using TaskManagement.Core.Models;

namespace TaskManagement.API.Controllers
{
    /// <summary>
    /// Users controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(string id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            user.UserId = Guid.NewGuid().ToString();
            var created = await _userRepository.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = created.UserId }, created);
        }
    }
}
