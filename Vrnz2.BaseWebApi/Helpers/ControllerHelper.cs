﻿using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Vrnz2.BaseContracts.DTOs.Base;
using Vrnz2.BaseInfra.MessageCodes;
using Vrnz2.BaseInfra.Validations;
using Vrnz2.BaseWebApi.CustomResults;
using Vrnz2.Infra.CrossCutting.Extensions;

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

        public async Task<ObjectResult> ReturnAsync<TRequest, TResult>(Func<TRequest, Task<TResult>> action, TRequest request, int successStatusCode = (int)HttpStatusCode.OK)
            where TRequest : BaseDTO.Request
            where TResult : BaseDTO.Response<TRequest>
        {
            try
            {
                var validation = _validationHelper.Validate(request);

                if (validation.IsValid)
                {
                    var response = await action(request);
                    
                    switch ((HttpStatusCode)response.StatusCode)
                    {
                        case HttpStatusCode statusCode when statusCode.IsSuccessHttpStatusCode():
                            return new OkObjectResult(response) { StatusCode = successStatusCode };
                        case HttpStatusCode.Unauthorized:
                            return new UnauthorizedObjectResult(response);
                        case HttpStatusCode.Forbidden:
                            return new ForbidenObjectResult(response);
                        default:
                            return GetBadRequestObjectResult(new List<string> { $"Unexpected error!" });
                    }
                }
                else if (validation.ErrorCodes.Contains(MessageCodesFactory.UNEXPECTED_ERROR))
                {
                    return GetInternalServerErrorObjectResult(validation.ErrorCodes);
                }
                else
                {
                    return GetBadRequestObjectResult(validation.ErrorCodes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error! - Message: {ex.Message}", ex);

                return GetBadRequestObjectResult(new List<string> { $"Unexpected error! - Message: {ex.Message}" });
            }
        }

        private InternalServerErrorObjectResult GetInternalServerErrorObjectResult(List<string> errorCodes) 
            => new InternalServerErrorObjectResult(errorCodes);

        private BadRequestObjectResult GetBadRequestObjectResult(List<string> errorCodes) 
            => new BadRequestObjectResult(errorCodes);

        #endregion
    }
}
