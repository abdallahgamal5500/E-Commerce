using E_Commerce.Core;
using E_Commerce.Core.Helpers;
using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;

namespace E_Commerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] User user) 
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!Validations.GetInstance().IsValidEmail(user.Email))
                return BadRequest($"{user.Email} is not a valid email");
            
            var result = await _unitOfWork.Users.FindSingle(u => u.Email == user.Email);

            if (result != null)
                return BadRequest($"{user.Email} is not a valid email");
            
            user.Role = user.Role.ToUpper();
          
            if (!Roles.ADMIN.Equals(user.Role.ToUpper()) && !Roles.CUSTOMER.Equals(user.Role.ToUpper()))
                return BadRequest($"{user.Role} is not a valid role");

            if (!Validations.GetInstance().IsValidPassword(user.Password))
                return BadRequest("invalid password");

            user.Password = await Encryptions.EncodePasswordToBase64(user.Password);

            _unitOfWork.Users.Add(user);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in creating new account");
           
            var response = new
            {
                token = await Encryptions.GenerateToken(user),
                isTokenChanged = true
            };
            
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (!Validations.GetInstance().IsValidEmail(loginDto.Email))
                return BadRequest($"{loginDto.Email} is not a valid email");

            if (!Validations.GetInstance().IsValidPassword(loginDto.Password))
                return BadRequest("invalid password");

            loginDto.Password = await Encryptions.EncodePasswordToBase64(loginDto.Password);

            var user = await _unitOfWork.Users
                .FindSingle(u => u.Email == loginDto.Email
                    && u.Password == loginDto.Password);

            if (user == null)
                return BadRequest("Incorrect email or password");

            var response = new
            {
                userId = user.Id,
                userName = user.Name,
                userEmail = user.Email,
                userRole = user.Role,
                userProfileImageUrl = user.ProfileImageUrl,
                token = await Encryptions.GenerateToken(user),
                isTokenChanged = true
            };

            return Ok(response);
        }
    }
}
