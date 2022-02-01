using Microsoft.AspNetCore.Mvc;
using Services.Common;

namespace API.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        public ActionResult Result<T>(ResponseService<T> response, string api ="")
            => response.StatusCode switch
            {
                Services.Common.StatusCodes.Ok => Ok(response),
                Services.Common.StatusCodes.Created => Created(api, response),
                Services.Common.StatusCodes.Accepted => Accepted(response),
                Services.Common.StatusCodes.BadRequest => BadRequest(response),
                Services.Common.StatusCodes.NotFound => NotFound(response),
                Services.Common.StatusCodes.Unauthorized => Unauthorized(response),
                Services.Common.StatusCodes.Forbidden => StatusCode(403, response),
                _ => StatusCode(500, response)
            };
    }
}