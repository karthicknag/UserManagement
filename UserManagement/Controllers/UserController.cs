using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly UserDbContext _dbContext;

        public UserController(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GetAllUsers

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if(_dbContext.Users == null)
            {
                return NotFound();
            }
            return await _dbContext.Users.ToListAsync();
        }
        // Get User based on Userid
        [HttpGet("UserId")]
        public async Task<ActionResult<User>> GetUser(Guid UserId)
        {
            if (_dbContext.Users == null)
            {
                return NotFound();
            }
            var user = await _dbContext.Users.FindAsync(UserId);
            if (user == null) { return NotFound(); }
            return user;
        }

        //Save User
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { UserId = user.UserId },user);
        }

        // Validate User credentials
        [HttpGet("{userName}/{password}")]
        public ActionResult<string> ValidateUser(string userName, string password)
        {
            string result = string.Empty;
            var user = _dbContext.Users.Where(e => e.UserName == userName);
            if(user == null) 
            { 
                result = "UserName is invalid, please enter valid username"; 
            } 
            else 
            {
              user = _dbContext.Users.Where(e => e.UserName == userName && e.Password == password);
                if (user == null)
                { 
                    result = "Password is invalid, please enter valid password"; 
                }
                else
                { 
                    result = "Sucess";
                }
            }
            return result;
        }
        // Update user details
        [HttpPut]
        public async Task<IActionResult> UpdateUser(Guid userId, User user)
        {
            if(userId != user.UserId)
            {
                return BadRequest();
            }
            
            try
            {
                _dbContext.Entry(user).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
