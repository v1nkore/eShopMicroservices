using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public abstract class ApiControllerBase : ControllerBase
	{
	}
}
