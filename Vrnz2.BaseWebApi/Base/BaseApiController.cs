using Vrnz2.Infra.CrossCutting.Extensions;
using Vrnz2.Security.Helpers.TokenHelper;
using Vrnz2.Security.Types;
using Microsoft.AspNetCore.Mvc;

namespace Vrnz2.BaseWebApi.Base
{
    public abstract class BaseApiController
        : ControllerBase
    {
        #region Variables

        protected readonly ITokenHelper _tokenHelper;

        #endregion

        #region Constructors

        protected BaseApiController(ITokenHelper tokenHelper)
            => _tokenHelper = tokenHelper;

        #endregion

        #region Methods

        protected string GetBearerToken()
        {
            var result = string.Empty;

            var authHeader = Request.Headers["Authorization"];

            if (authHeader.HaveAny())
                result = authHeader[0].Replace("Bearer ", string.Empty);

            return result;
        }

        protected string GetLocale()
        {
            var token = GetBearerToken();

            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            return _tokenHelper.GetClaimValue(GetBearerToken(), JwtRegisteredClaimNamesExtended.Locale);
        }

        protected string GetWorkflowId()
        {
            var token = GetBearerToken();

            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            return _tokenHelper.GetClaimValue(GetBearerToken(), JwtRegisteredClaimNamesExtended.IdMain);
        }

        #endregion
    }
}
