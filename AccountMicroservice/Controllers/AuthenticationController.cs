using AccountMicroservice.DBContexts;
using AccountMicroservice.Models;
using AccountMicroservice.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AccountContext _dbcontext;
        private readonly ITokenBuilder _tokenBuilder;

        public AuthenticationController(
            AccountContext dbcontext,
            ITokenBuilder tokenBuilder)
        {
            _dbcontext = dbcontext;
            _tokenBuilder = tokenBuilder;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var dbUser = await _dbcontext
                .Users
                .SingleOrDefaultAsync(u => u.UserName == user.UserName);

            if (dbUser == null)
            {
                return NotFound("User not found.");
            }

            // This is just an example, made for simplicity; do not store plain passwords in the database
            // Always hash and salt your passwords
            var isValid = dbUser.PwdHash == user.PwdHash;

            if (!isValid)
            {
                return BadRequest("Could not authenticate user.");
            }

            var token = _tokenBuilder.BuildToken(user.UserName);

            return Ok(token);
        }


        [HttpGet("verify")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> VerifyToken()
        {
            var username = User
                .Claims
                .SingleOrDefault();

            if (username == null)
            {
                return Unauthorized();
            }

            var userExists = await _dbcontext
                .Users
                .AnyAsync(u => u.UserName == username.Value);

            if (!userExists)
            {
                return Unauthorized();
            }

            //var userResponse = await _dbcontext
            //                .UserProfileView
            //                .Where(u => u.UserName == username.Value)
            //                .Select(s => new { UserId = s.UserId, UserName = s.UserName, RoleId = s.RoleId}).FirstOrDefaultAsync();

            //return new OkObjectResult(userResponse);
            return NoContent();
        }

        [HttpGet("GetUserByUserName/{username}")]
        public async Task<IActionResult> GetUserIdByUserName(string username)
        {
            var profile = await _dbcontext.UserProfileView.FirstOrDefaultAsync(u => u.UserName == username);

            return new OkObjectResult(profile);
        }


        [HttpGet("GetMyProfile/{userId}")]
        public async Task<IActionResult> GetMyProfile(string userId)
        {
            var profile = await _dbcontext.UserProfileView.FirstOrDefaultAsync(u => u.UserId == userId);

            return new OkObjectResult(profile);
        }

        [HttpGet("GetEmailsByRoleId/{roleId}")]
        public async Task<IActionResult> GetEmails(string roleId)
        {
            var profile = await _dbcontext.UserProfileView.Where (s => s.RoleId == roleId).Select( s => new { Name = s.UserName, Role = s.RoleName, EmailAddr = s.Email }).ToListAsync();

            return new OkObjectResult(profile);
        }

        [HttpGet("GetEmailByUserId/{userId}")]
        public async Task<IActionResult> GetEmailByUserId(string userId)
        {
            var profile = await _dbcontext.UserProfileView.Where(s => s.UserId == userId).Select(s => new { Name = s.UserName, EmailAddr = s.Email }).ToListAsync();

            return new OkObjectResult(profile);
        }

        [HttpGet("GetUserMenu/{userId}")]
        public async Task<IActionResult> GetUserMenu(string userId)
        {
            var profile = await _dbcontext.UserProfileView.Where(s => s.UserId == userId).Select(s => new { Name = s.UserName, Role = s.RoleName,EmailAddr = s.Email,HomePhone=s.HomePhone, OfficePhone=s.OfficePhone}).ToListAsync();

            return new OkObjectResult(profile);
        }
        [HttpGet("GetUserRole/{userId}")]
        public async Task<IActionResult> GetUserRole(string userId)
        {
            var roleId = await _dbcontext.User_Roles.FirstOrDefaultAsync(u => u.UserId == userId);

            var Role = await _dbcontext.Role.SingleOrDefaultAsync(u => u.RoleId == roleId.RoleId);

            return new OkObjectResult(Role);
        }

        [HttpGet("GetAllRoles")]
        public async Task<List<Role>> GetRoleDrop()
        {
            var Role = await _dbcontext.Role.OrderBy(c => c.RoleName).ToListAsync();

            return Role;
        }
        [HttpGet("GetAllUser")]
        public async Task<List<UserProfileView>> GetUsers()
        {
            var profile = await _dbcontext.UserProfileView.OrderBy(c => c.UserId).ToListAsync();

            return profile;
        }

        [Route("CreateRole")]
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] Role roleToAdd)
        {
            var role = await _dbcontext.Role.FirstOrDefaultAsync(u => u.RoleName == roleToAdd.RoleName);

            if (role != null)
            {
                return NotFound(new { Message = $"role already exist" });
            }

            roleToAdd.RoleId = Guid.NewGuid().ToString();
            roleToAdd.UpdateDate = DateTime.Now;
            roleToAdd.CreatedDate = DateTime.Now;

            _dbcontext.Role.Add(roleToAdd);

            await _dbcontext.SaveChangesAsync();

            return Ok();
        }
        //Edit role
        [Route("editrole")]
        [HttpPost]
        public async Task<IActionResult> EditRole([FromBody] Role roleToEdit)
        {
            var role = await _dbcontext.Role.FirstOrDefaultAsync(u => u.RoleId == roleToEdit.RoleId);

            if (role == null)
            {
                return NotFound(new { Message = $"role does not exist" });
            }

            role = roleToEdit;
            role.UpdateDate = DateTime.Now;

            _dbcontext.Role.Update(role);

            await _dbcontext.SaveChangesAsync();

            return Ok();
        }
        //Create user
        [Route("createUser")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] AddUser user)
        {
            //Check if username exist
            var username = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);

            if (username != null)
            {
                return NotFound(new { Message = $"Username already exist" });
            }

            User newUser = new User()
            {
                UserId = Guid.NewGuid().ToString(),
                UserName = user.UserName,
                CreatedBy = user.CreatedBy,
                CreatedDate = DateTime.Now,
                UpdatedBy = user.CreatedBy,
                UpdateDate = DateTime.Now,
                PwdHash = user.PwdHash,
                Disable = "0"
            };

            _dbcontext.Users.Add(newUser);
            await _dbcontext.SaveChangesAsync();

            User_Info new_User_Info = new User_Info()
            {
                UserId = newUser.UserId,
                UserinfoId = newUser.UserId,
                HomePhone = user.HomePhone,
                OfficePhone = user.OfficePhone,
                Email =user.Email,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                UpdatedBy = user.CreatedBy,
                CreatedBY = user.CreatedBy
            };

            //Add new user info
            _dbcontext.User_Info.Add(new_User_Info);
            await _dbcontext.SaveChangesAsync();


            //Assign role
            //Check if username exist
            var role = await _dbcontext.Role.FirstOrDefaultAsync(u => u.RoleName == user.roleName);

            User_Role addUserRole = new User_Role()
            {
                UserId = newUser.UserId,
                RoleId = role.RoleId,
                CreatedBy = user.CreatedBy,
                CreatedDate = DateTime.Now,
                UpdateBy = user.CreatedBy,               
                UpdateDate = DateTime.Now,

            };

            _dbcontext.User_Roles.Add(addUserRole);

            await _dbcontext.SaveChangesAsync();


            return Ok();
        }
        [Route("editUser")]
        [HttpPost]
        //Edit user
        public async Task<IActionResult> EditUser([FromBody] EditUser userToUpdate)
        {
            //Find user name
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserId == userToUpdate.UserId);

            if (user == null)
            {
                return NotFound(new { Message = $"Username does not exist" });
            }

            user.UserName = userToUpdate.UserName;
            user.PwdHash = userToUpdate.PwdHash;
            user.Disable = userToUpdate.Disable;
            user.UpdateDate = DateTime.Now;
            user.UpdatedBy = userToUpdate.UpdatedBy;

            _dbcontext.Users.Update(user);

            var user_info = await _dbcontext.User_Info.FirstOrDefaultAsync(u => u.UserId == userToUpdate.UserId);

            if (user_info == null)
            {
                return NotFound(new { Message = $"UserInfo does not exist" });
            }

            user_info.HomePhone = userToUpdate.HomePhone;
            user_info.OfficePhone = userToUpdate.OfficePhone;
            user_info.UpdatedDate = DateTime.Now;
            user_info.UpdatedBy = userToUpdate.UpdatedBy;
            user_info.Email = userToUpdate.Email;

            _dbcontext.User_Info.Update(user_info);

            await _dbcontext.SaveChangesAsync();


            //Assign role
            //Check if username exist
            var userrole = await _dbcontext.User_Roles.FirstOrDefaultAsync(u => u.UserId == userToUpdate.UserId);

            var role = await _dbcontext.Role.FirstOrDefaultAsync(u => u.RoleName.Equals(userToUpdate.roleName));

            userrole.UpdateDate = DateTime.Now;
            userrole.RoleId = role.RoleId;
            userrole.UpdateBy = userToUpdate.UpdatedBy;

            _dbcontext.User_Roles.Update(userrole);
            await _dbcontext.SaveChangesAsync();
            //Return new object
            return Ok();
        }
        private List<Role> ChangeRoleDropDown(List<Role> items)
        {

            return items;
        }


    }
}
