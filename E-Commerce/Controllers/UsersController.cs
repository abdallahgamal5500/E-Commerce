using E_Commerce.Core;
using E_Commerce.Core.Helpers;
using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace E_Commerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            var result = await _unitOfWork.Users.GetAll(user => new
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            });

            var response = new
            {
                result = result,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };

            return Ok(response);
        }
        
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById([BindRequired] int id, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            var user = await _unitOfWork.Users.GetById(id);
            if (user == null)
                return NotFound($"Not found user with this id: {id}");

            var response = new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
        
        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] EditUserDto editedUser, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (!Validations.GetInstance().IsValidEmail(editedUser.Email))
                return BadRequest($"{editedUser.Email} is not a valid email");

            if (!Validations.GetInstance().IsValidPassword(editedUser.Password))
                return BadRequest("invalid password");

            if (!editedUser.Password.Equals(string.Empty) && Validations.GetInstance().IsValidPassword(editedUser.Password))

                validatingUserToken.User.Password = await Encryptions.EncodePasswordToBase64(editedUser.Password);

            else if (!Validations.GetInstance().IsValidPassword(editedUser.Password))
                return BadRequest("invalid password");

            validatingUserToken.User.Name = editedUser.Name;
            validatingUserToken.User.Email = editedUser.Email;
            validatingUserToken.User.ProfileImageUrl = editedUser.ProfileImageUrl;

            _unitOfWork.Users.Update(validatingUserToken.User);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in updateing the account to database");

            var response = new
            {
                userId = validatingUserToken.User.Id,
                userName = validatingUserToken.User.Name,
                userEmail = validatingUserToken.User.Email,
                userRole = validatingUserToken.User.Role,
                userProfileImageUrl = validatingUserToken.User.ProfileImageUrl,
                token = await Encryptions.GenerateToken(validatingUserToken.User),
                isTokenChanged = true
            };

            return Ok(response);
        }
        [HttpDelete("DeleteUserById")]
        public async Task<IActionResult> DeleteUserById([BindRequired] int id, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            var user = await _unitOfWork.Users.GetById(id);
            if (user == null)
                return NotFound($"Not found user with this id: {id}");

            _unitOfWork.Users.Delete(user);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in removing user from database");

            var response = new
            {
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }    
    }
}
