using Microsoft.AspNetCore.Mvc;
using Services.Common;

namespace API.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public ActionResult Result<T>(ResponseService<T> response, string api = "")
            => response.Status switch
            {
                "Ok" => Ok(response),
                "Created" => Created(api, response),
                "Accepted" => Accepted(response),
                "BadRequest" => BadRequest(response),
                "NotFound" => NotFound(response),
                "Unauthorized" => Unauthorized(response),
                "Forbidden" => StatusCode(403, response),
                _ => StatusCode(500, response)
            };
    }
}