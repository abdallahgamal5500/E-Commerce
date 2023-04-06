using E_Commerce.Core;
using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Helpers;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace E_Commerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([BindRequired] string name, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (name.Equals(string.Empty))
                return BadRequest("please write a valid name");

            var category = new Category();
            category.Name = name;

            _unitOfWork.Categories.Add(category);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in adding new category to the database");

            var response = new
            {
                categoryId = category.Id,
                categoryName = category.Name,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            if (!ModelState.IsValid)
                return BadRequest("");

            return Ok(await _unitOfWork.Categories.GetAll());
        }
        [HttpGet("GetCategoryById")]
        public async Task<IActionResult> GetCategoryById([BindRequired] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest("");

            if (id <= 0)
                return BadRequest("invalid Id");

            var category = await _unitOfWork.Categories.GetById(id);
            if (category == null)
                return NotFound("This category is not found");
            
            return Ok(category);
        }
        [HttpPut("EditCategoryById")]
        public async Task<IActionResult> EditCategoryById([BindRequired] int id,
            [BindRequired] string name, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (id <= 0)
                return BadRequest("Please write a valid id");

            if (name.Equals(string.Empty))
                return BadRequest("please write a valid name");

            var category = await _unitOfWork.Categories.GetById(id);
            if (category == null)
                return NotFound("Not found this category");

            category.Name = name;
            _unitOfWork.Categories.Update(category);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in updating the category to the database");

            var response = new
            {
                categoryId = category.Id,
                categoryName = category.Name,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
        [HttpDelete("DeleteCategoryById")]
        public async Task<IActionResult> DeleteCategoryById([BindRequired] int id, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (id <= 0)
                return BadRequest("Please write a valid id");

            var category = await _unitOfWork.Categories.GetById(id);
            if (category == null)
                return NotFound("Not found this category");

            _unitOfWork.Categories.Delete(category);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in deleting the category from the database");

            var response = new
            {
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
    }
}
