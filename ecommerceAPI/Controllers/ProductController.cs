using ecommerceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ecommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ProductController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public IActionResult GetAll()
        {
           var products = dbContext.Product.Include(r=>r.reviews).ToList();

            return Ok(products);
        }

       [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var product = dbContext.Product.Include(r => r.reviews).FirstOrDefault(p => p.Id == id);

            return Ok(product);

        }

        [HttpGet("{category}")]
        public IActionResult GetByCategory(string category)

        {
            var product = dbContext.Product.Include(r => r.reviews).Where(p => p.category == category);

            return Ok(product);
        }
    }
}
