using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Common;

namespace API.Controllers.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {

        protected readonly IUnitOfWork _unitOfWork;
        public BaseController(IUnitOfWork _unitOfWork){
            this._unitOfWork=_unitOfWork;
        }
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