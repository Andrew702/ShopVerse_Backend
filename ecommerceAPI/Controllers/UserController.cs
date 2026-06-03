using ecommerceAPI.Data;
using ecommerceAPI.DTO;
using ecommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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


        //[HttpGet]
        //public IActionResult Login(LoginDTO loginDTO)
        //{

        //}

    }
}
