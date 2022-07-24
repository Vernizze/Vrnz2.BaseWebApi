using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace Vrnz2.BaseWebApi.CustomResults
{
    public class NoContentObjectResult
        : ObjectResult
    {
        public NoContentObjectResult(ModelStateDictionary modelState)
            : base(modelState)
        {
            StatusCode = (int)HttpStatusCode.NoContent;
        }

        public NoContentObjectResult(object error)
            : base(error)
        {
            StatusCode = (int)HttpStatusCode.NoContent;
        }
    }
}
