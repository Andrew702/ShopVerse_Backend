using ecommerceAPI.Data;
using ecommerceAPI.DTO;
using ecommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly UserManager<User> userManager;

        public UserController(AppDbContext dbContext,UserManager<User> userManager) 
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var user = new User
            {
                Id = registerDTO.Id,
                UserName=registerDTO.UserName,
                Email=registerDTO.Email,
                PhoneNumber = registerDTO.Phone
            };
            var result = await userManager.CreateAsync(user,registerDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.ToList()[0].Description);
            }
            return Ok(registerDTO);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user =await userManager.FindByEmailAsync(loginDTO.Email);
            if (user==null)
            {
                return NotFound("email or password is incorrect");
            }
            var password = await userManager.CheckPasswordAsync(user,loginDTO.Password);

            if (!password)
            {
                return NotFound("email or password is incorrect");
            }

            var userRet = dbContext.Users.Include(o => o.orders).Include(c => c.CartItems).Include(w => w.Wishlists).FirstOrDefault(u => u.Id == user.Id);

            UserDTO userDTO = new UserDTO
            {
                Id = userRet.Id,
                UserName = userRet.UserName,
                Email=userRet.Email,
                Phone=userRet.PhoneNumber,
                orders=userRet.orders,
                cartItems=userRet.CartItems,
                wishlists=userRet.Wishlists

            };
            return Ok(userDTO);
        }


        [HttpPost("Wishlist")]
        public async Task<IActionResult> AddToWishlist(int PID, string UID)
        {
            //search for user by UID
            var userRet = dbContext.Users.FirstOrDefault(u => u.Id == UID);
            //Add to user's wishlist
            if(userRet != null)
            {
                dbContext.Wishlists.Add(new()
                {
                    ProductId = PID,
                    UserId = UID
                });
                dbContext.SaveChanges();
                return Ok(userRet);
            }
            return BadRequest("Failed to add to wishlist");
        }



        [HttpPost("Cart")]
        public async Task<IActionResult> AddtoCart(string UID, int PID, int QTY, string CartID)
        {
            //search for user by UID
            var userRet = dbContext.Users.FirstOrDefault(u => u.Id == UID);
            //Add to user's wishlist
            if (userRet != null)
            {
                dbContext.CartItems.Add(new()
                {
                    id = CartID,
                    ProductId = PID,
                    UserId = UID,
                    quantity = QTY
                });
                dbContext.SaveChanges();
                return Ok(userRet);
            }
            return BadRequest("Failed to add to cart");
        }


        [HttpPost("Order")]
        public async Task<IActionResult> MakeOrder(string UID,string orderId)
        {
            var user = dbContext.Users.FirstOrDefault(u=>u.Id == UID);

            if(user == null)
            {
                return NotFound("User not found");
            }
            var cart = dbContext.CartItems.Include(c => c.Product).Where(u=>u.UserId==UID).ToList();

            decimal total = cart.Sum(item => item.Product.price * item.quantity);

            var order = new Order
            {
                id=orderId,
                UserId = UID,
                total = total,
                date = DateTime.Now
            };

            dbContext.CartItems.RemoveRange(cart);

            foreach (var item in cart)
            {
                dbContext.OrderItems.Add(new()
                {
                    id = item.id,
                    orderId = orderId,
                    ProductId = item.ProductId,
                    quantity = item.quantity
                });
            }
            dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            var orderRet = dbContext.Orders.Include(o=>o.OrderItems).FirstOrDefault(o=>o.id== orderId);
            return Ok(orderRet);
        }



    }
}
