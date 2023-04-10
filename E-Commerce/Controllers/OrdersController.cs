using E_Commerce.Core.Helpers.Constants;
using E_Commerce.Core.Helpers;
using E_Commerce.Core.Models.Database;
using E_Commerce.Core.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Xml.Linq;
using System.Net;
using E_Commerce.EF;

namespace E_Commerce.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpPost("AddOrder")]
        public async Task<ActionResult> AddOrder([BindRequired] string address, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.CUSTOMER);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            if (address.Equals(string.Empty))
                return BadRequest("please add a valid address");

            var entities = await _unitOfWork.CartItems
                .GetCartItemsDetailsByUserId(validatingUserToken.User);

            if (entities.Count() == 0)
                return BadRequest("your cart is empty. you can't make an order");

            var order = new Order
            {
                UserId = validatingUserToken.User.Id,
                Address = address,
                TotalCost = entities.Sum
                    (item => item.CartItem.Quantity * item.Product.Price)
            };

            _unitOfWork.Orders.Add(order);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in adding new order to the database");

            IEnumerable<OrderItem> orderItems = entities
                .Select(item => new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.Product.Id,
                    Quantity = item.CartItem.Quantity
                });

            _unitOfWork.OrdersItems.AddRange(orderItems);
            if (await _unitOfWork.Complete() < 1)
            {
                _unitOfWork.Orders.Delete(order);
                if (await _unitOfWork.Complete() < 1)
                    BadRequest("Error in removing the order from the database");
                BadRequest("Error in adding the order items to the database");
            }
   
            _unitOfWork.CartItems.Delete(entities
                .Select(item => item.CartItem));
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in deleting the cart items from the database");

            var response = new
            {
                orderId = order.Id,
                orderAddress = order.Address,
                orderTotalCost = order.TotalCost,
                orderStatus = order.Status,
                orderDate = order.OrderDate,
                orderDeliveryDate = order.DeliveryDate,
                userId = validatingUserToken.User.Id,
                userName = validatingUserToken.User.Name,
                userEmail = validatingUserToken.User.Email,
                userRole = validatingUserToken.User.Role,
                products = entities.Select(entity => new
                {
                    productId = entity.Product.Id,
                    productName = entity.Product.Name,
                    productDescription = entity.Product.Description,
                    productPrice = entity.Product.Price,
                    productImageUrl = entity.Product.imageUrl,
                    productColor = entity.Product.Color,
                    productSize = entity.Product.Size,
                    categoryId = entity.Category.Id,
                    categoryName = entity.Category.Name,
                    quantity = entity.CartItem.Quantity,
                }).ToList(),
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response); 
        }

        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders([FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            IEnumerable<Order> orders;

            if (validatingUserToken.User.Role.Equals(Roles.ADMIN))
                orders = await _unitOfWork.Orders.GetAll();
            else
                orders = await _unitOfWork.Orders
                    .FindAll(o => o.UserId == validatingUserToken.User.Id);

            var result = orders.Select(o => new
            {
                orderId = o.Id,
                orderAddress = o.Address,
                orderTotalCost = o.TotalCost,
                orderStatus = o.Status,
                orderDate = o.OrderDate,
                orderDeliveryDate = o.DeliveryDate,
            });

            var response = new
            {
                orders = result,
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };

            return Ok(response);
        }
        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById([BindRequired] int id, [FromHeader] string token)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            var entities = await _unitOfWork.Orders
                .GetOrderDetailsById(id, validatingUserToken.User);

            var result = entities
                .FirstOrDefault(o => o.Order.Id == id);

            if (result == null)
                return NotFound($"Not found order with this id: {id}");

            var order = result.Order;
            var user = result.User;
            
            var response = new
            {
                orderId = order.Id,
                orderAddress = order.Address,
                orderTotalCost = order.TotalCost,
                orderStatus = order.Status,
                orderDate = order.OrderDate,
                orderDeliveryDate = order.DeliveryDate,
                userId = user.Id,
                userName = user.Name,
                userEmail = user.Email,
                userRole = user.Role,
                products = entities.Select(entity => new
                {
                    productId = entity.Product.Id,
                    productName = entity.Product.Name,
                    productDescription = entity.Product.Description,
                    productPrice = entity.Product.Price,
                    productImageUrl = entity.Product.imageUrl,
                    productColor = entity.Product.Color,
                    productSize = entity.Product.Size,
                    categoryId = entity.Category.Id,
                    categoryName = entity.Category.Name,
                }).ToList(),
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
        
        [HttpPut("EditOrderStatusById")]
        public async Task<IActionResult> EditOrderStatusById(int id, string status, [FromHeader] string token)
        {
            if (status.Equals(string.Empty) || 
                (!status.Equals(OrdersStatuses.PENDING) && 
                !status.Equals(OrdersStatuses.SHIPPED) &&
                !status.Equals(OrdersStatuses.DELIVERED)))
                return BadRequest("In valid status");

            if (!ModelState.IsValid)
                return BadRequest();

            var validatingUserToken = await Validations.GetInstance()
                .ValidateUserToken(token, _unitOfWork, Roles.ADMIN);

            if (validatingUserToken.StatusCode == 401)
                return Unauthorized("Unauthorized");

            var order = await _unitOfWork.Orders.GetById(id);
            order.Status = status;

            _unitOfWork.Orders.Update(order);
            if (await _unitOfWork.Complete() < 1)
                BadRequest("Error in updateing the order state to database");

            var response = new
            {
                token = validatingUserToken.Token,
                isTokenChanged = validatingUserToken.IsTokenChanged
            };
            return Ok(response);
        }
    }
}