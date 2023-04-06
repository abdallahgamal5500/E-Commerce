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
    public class CartItemsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpPost("AddCartItem")]
        public async Task<ActionResult> AddCartItem([FromBody] CartItemDto cartItemDto, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.CUSTOMER);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (cartItemDto.ProductId <= 0)
                return BadRequest("please select a valid product");

            if (cartItemDto.Quantity <= 0)
                return BadRequest("please select a correct quantity");

            var product = await _unitOfWork.Products
                .FindSingle(p => p.Id == cartItemDto.ProductId, new[] { "Category" });

            if (product == null)
                return BadRequest("please select a valid product");

            var existCartItem = await _unitOfWork.CartItems
                .FindSingle(cartItem => cartItem.UserId == validatingUserToken.User.Id
                    && cartItem.ProductId == cartItemDto.ProductId);

            int id, quantity;
            decimal totalCost;

            if (existCartItem != null)
            {
                existCartItem.Quantity += cartItemDto.Quantity;
                _unitOfWork.CartItems.Update(existCartItem);
                if (await _unitOfWork.Complete() < 1)
                    BadRequest("Error in adding new cart item to the database");

                id = existCartItem.Id;
                quantity = existCartItem.Quantity;
                totalCost = existCartItem.Quantity * product.Price;
            }
            else
            {
                var cartItem = new CartItem
                {
                    Product = product,
                    User = validatingUserToken.User,
                    Quantity = cartItemDto.Quantity
                };

                _unitOfWork.CartItems.Add(cartItem);
                if (await _unitOfWork.Complete() < 1)
                    BadRequest("Error in adding new cart item to the database");

                id = cartItem.Id;
                quantity = cartItem.Quantity;
                totalCost = cartItem.Quantity * product.Price;
            }

            var response = new
            {
                cartItemId = id,
                productId = product.Id,
                name = product.Name,
                description = product.Description,
                price = product.Price,
                imageUrl = product.imageUrl,
                color = product.Color,
                size = product.Size,
                categoryId = product.CategoryId,
                categoryName = product.Category.Name,
                quantity = quantity,
                totalCost = totalCost,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };

            return Ok(response);
        }
        [HttpPut("EditCartItem")]
        public async Task<ActionResult> EditCartItem([BindRequired] int id, [BindRequired] int quantity, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.CUSTOMER);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (quantity <= 0)
                return BadRequest("please select a correct quantity");

            var cartItem = await _unitOfWork.CartItems.GetById(id);
            if (cartItem == null)
                return NotFound($"not found cart item with this id: {id}");

            cartItem.Quantity = quantity;
            _unitOfWork.CartItems.Update(cartItem);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in updating the cart item to database");

            var response = new
            {
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
        [HttpDelete("DeleteCartItemById")]
        public async Task<IActionResult> DeleteCartItemById([BindRequired] int id, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.CUSTOMER);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            var cartItem = await _unitOfWork.CartItems.GetById(id);
            if (cartItem == null)
                return NotFound($"not found cart item with this id: {id}");

            _unitOfWork.CartItems.Delete(cartItem);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in removing cart item from database");

            var response = new
            {
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
    }
}
