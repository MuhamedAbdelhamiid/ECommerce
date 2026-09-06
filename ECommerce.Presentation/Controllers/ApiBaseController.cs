using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Shared.CommonResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ECommerce.Presentation.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected ActionResult HandleResult(Result resultObject)
        {
            if (resultObject.IsSuccess)
                return NoContent();
            else
                return HandleProblem(resultObject.Errors);
        }

        protected ActionResult HandleResult<TValue>(Result<TValue> resultObject)
        {
            if (resultObject.IsSuccess)
                return Ok(resultObject.Data);
            else
                return HandleProblem(resultObject.Errors);
        }

        private ActionResult HandleProblem(IEnumerable<Error> errors)
        {
            if (errors.Count() == 0)
            {
                // came with falied without any errors
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "An Error Occured"
                );
            }

            if (errors.All(E => E.Type == ErrorTypes.Validation))
                return HandleValidationErrors(errors);

            return HandleSingleError(errors.FirstOrDefault()!);
        }

        private ActionResult HandleSingleError(Error error)
        {
            return Problem(
                title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                statusCode: MapStatusCode(error.Type)
            );
        }

        private ActionResult HandleValidationErrors(IEnumerable<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }

            return ValidationProblem(modelState);
        }

        private static int MapStatusCode(ErrorTypes errorType) =>
            errorType switch
            {
                ErrorTypes.Validation => StatusCodes.Status400BadRequest,
                ErrorTypes.NotFound => StatusCodes.Status404NotFound,
                ErrorTypes.UnAuthorized => StatusCodes.Status401Unauthorized,
                ErrorTypes.Forbidden => StatusCodes.Status403Forbidden,
                ErrorTypes.InvalidCredintials => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError,
            };
    }
}
