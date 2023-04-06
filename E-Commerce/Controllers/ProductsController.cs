using E_Commerce.Core;
using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Helpers;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Drawing;
using System.Xml.Linq;

namespace E_Commerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("AddProduct")]
        public async Task<ActionResult> AddProduct([FromBody] ProductDto productDto, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (productDto.Name.Equals(string.Empty)
                || productDto.Description.Equals(string.Empty)
                || productDto.imageUrl.Equals(string.Empty)
                || productDto.Name.Equals(string.Empty))
                return BadRequest("This field can't be empty");

            if (productDto.Price <= 0)
                return BadRequest("please enter a valid price");

            if (productDto.CategoryId <= 0)
                return BadRequest("please select a valid category");

            var category = await _unitOfWork.Categories.GetById(productDto.CategoryId);
            if (category == null) 
                return BadRequest("please select a valid category");

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                imageUrl = productDto.imageUrl,
                Color = productDto.Color,
                Size = productDto.Size,
                Category = category
            };

            _unitOfWork.Products.Add(product);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in adding new product to database");

            var response = new
            {
                id = product.Id,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                imageUrl = product.imageUrl,
                color = product.Color,
                size = product.Size,
                categoryId = product.CategoryId,
                categoryName = product.Category.Name,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };

            return Ok(response);
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var products = await _unitOfWork.Products.GetAll(new[] { "Category" });
            if (products == null)
                return NotFound("not found products");

            var response = products.Select(p => new 
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                price = p.Price,
                imageUrl = p.imageUrl,
                color = p.Color,
                size = p.Size,
                categoryId = p.CategoryId,
                categoryName = p.Category.Name,
            });

            return Ok(response);
        }

        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById([BindRequired] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var product = await _unitOfWork.Products.FindSingle(p=> p.Id == id, new[] { "Category" });
            if (product == null)
                return NotFound($"not found product with this id: {id}");

            var response = new
            {
                id = product.Id,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                imageUrl = product.imageUrl,
                color = product.Color,
                size = product.Size,
                categoryId = product.Category.Id,
                categoryName = product.Category.Name,
            };
            return Ok(response);
        }
        [HttpPut("EditProduct")]
        public async Task<ActionResult> EditProduct([BindRequired] int id, [FromBody] ProductDto productDto, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");
            
            if (productDto.Name.Equals(string.Empty)
                || productDto.Description.Equals(string.Empty)
                || productDto.imageUrl.Equals(string.Empty)
                || productDto.Name.Equals(string.Empty))
                return BadRequest("These fields can't be empty");

            if (productDto.Price <= 0)
                return BadRequest("please enter a valid price");

            if (productDto.CategoryId <= 0)
                return BadRequest("please select a valid category");

            var category = await _unitOfWork.Categories.GetById(productDto.CategoryId);
            if (category == null)
                return BadRequest("please select a valid category");

            var product = await _unitOfWork.Products.FindSingle(p => p.Id == id, new[] { "Category" });
            if (product == null)
                return NotFound($"not found product with this id: {id}");

            product.Id = id;
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.imageUrl = productDto.imageUrl;
            product.Color = productDto.Color;
            product.Size = productDto.Size;
            product.Category = category;
 
            _unitOfWork.Products.Update(product);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error updating the product to database");

            var response = new
            {
                id = product.Id,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                imageUrl = product.imageUrl,
                color = product.Color,
                size = product.Size,
                categoryId = product.CategoryId,
                categoryName = product.Category.Name,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };

            return Ok(response);
        }
        [HttpDelete("DeleteProductById")]
        public async Task<IActionResult> DeleteProductById([BindRequired] int id, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            var product = await _unitOfWork.Products.FindSingle(p => p.Id == id, new[] { "Category" });
            if (product == null)
                return NotFound($"Not found product with this id: {id}");

            _unitOfWork.Products.Delete(product);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in removing product from database");

            var response = new
            {
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
    }
}
