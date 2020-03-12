using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApi.DTOs;

namespace WebApi.Controllers
{
  [Route("api/[controller]")]
  public class WebApiController : ControllerBase
  {
    [HttpGet]
    [ProducesResponseType(typeof(ResultDTO), 200)]
    public ActionResult<string> HelloWorld()
    {
      return Ok("Hello World!!");
    }
  }
}

