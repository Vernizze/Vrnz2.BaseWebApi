using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Vrnz2.BaseContracts.DTOs;
using Vrnz2.BaseWebApi.Helpers;

namespace Vrnz2.BaseWebApi.Controllers
{
    [ExcludeFromCodeCoverage]
    [ApiController]
    [Route("")]
    public class PingController
        : ControllerBase
    {
        #region Methods

        /// <summary>
        /// Service Heart Beat EndPoint
        /// </summary>
        /// <returns>DateTime.UtcNow + Service Name</returns>
        [HttpGet("ping")]
        [AllowAnonymous]
        public async Task<ObjectResult> Ping([FromServices] IControllerHelper controllerHelper)
            => await controllerHelper.ReturnAsync<Ping.Request, Ping.Response, PingResponse>((request) => Task.FromResult(new Ping.Response { Success = true, StatusCode = (int)HttpStatusCode.OK }), new Ping.Request());

        #endregion
    }
}
