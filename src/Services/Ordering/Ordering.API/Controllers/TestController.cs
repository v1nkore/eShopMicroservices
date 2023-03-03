using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;

namespace Ordering.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class TestController : ControllerBase
	{
		private readonly OrderContext _dbContext;

		public TestController(OrderContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsync()
		{
			return Ok(await _dbContext.Orders.FirstAsync());
		}

		[HttpGet("all")]
		public async Task<IActionResult> GetAllAsync()
		{
			return Ok(await _dbContext.Orders.ToListAsync());
		}
	}
}