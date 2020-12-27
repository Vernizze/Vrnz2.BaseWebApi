using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vrnz2.BaseContracts.DTOs.Base;
using Vrnz2.BaseInfra.MessageCodes;
using Vrnz2.BaseInfra.Validations;
using Vrnz2.BaseWebApi.CustomResults;

namespace Vrnz2.BaseWebApi.Helpers
{
    public class ControllerHelper
    {
        #region Variables

        private readonly ILogger _logger;
        private readonly ValidationHelper _validationHelper;

        #endregion

        #region Constructors

        public ControllerHelper(ILogger logger, ValidationHelper validationHelper)
        {
            _logger = logger;
            _validationHelper = validationHelper;
        }

        #endregion

        #region Methods

        public async Task<ObjectResult> ReturnAsync<TRequest, TResult>(Func<TRequest, Task<TResult>> action, TRequest request, int? successStatusCode)
            where TRequest : BaseDTO.Request
        {
            try
            {
                var validation = _validationHelper.Validate(request);

                if (validation.IsValid)
                {
                    var response = await action(request);

                    var result = new OkObjectResult(response);

                    if (successStatusCode.HasValue)
                        result.StatusCode = successStatusCode;

                    return result;
                }
                else if (validation.ErrorCodes.Contains(MessageCodesFactory.UNEXPECTED_ERROR))
                {
                    return new InternalServerErrorObjectResult(validation.ErrorCodes);
                }
                else
                {
                    return new BadRequestObjectResult(validation.ErrorCodes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error! - Message: {ex.Message}", ex);

                return new InternalServerErrorObjectResult(new List<string> { $"Unexpected error! - Message: {ex.Message}" });
            }
        }

        #endregion
    }
}
