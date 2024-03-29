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
using Vrnz2.BaseWebApi.DTOs;
using Vrnz2.Infra.CrossCutting.Extensions;

namespace Vrnz2.BaseWebApi.Helpers
{
    public class ControllerHelper
        : IControllerHelper
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

        public async Task<ObjectResult> ReturnAsync<TRequest, TResult, T>(Func<TRequest, Task<TResult>> action, TRequest request, int successStatusCode = (int)HttpStatusCode.OK, bool ignoreMessageCodesFactory = false)
            where TRequest : BaseDTO.Request
            where TResult : BaseDTO.Response<T>
        {
            try
            {
                var validation = _validationHelper.Validate(request, ignoreMessageCodesFactory);

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
                        case HttpStatusCode.NotFound:
                            return new NotFoundObjectResult(response);
                        case HttpStatusCode.NoContent:
                            return new NoContentObjectResult(response);
                        case HttpStatusCode.BadRequest:
                            return new BadRequestObjectResult(response);
                        default:
                            return GetBadRequestObjectResult(new List<string> { response.Message });
                    }
                }
                else if (validation.ErrorCodes.Contains(MessageCodesFactory.UNEXPECTED_ERROR))
                {
                    return GetInternalServerErrorObjectResult(new InternalServerErrorResultDto(validation.ErrorCodes));
                }
                else
                {
                    return GetBadRequestObjectResult(new BadRequestObjectResultDto(validation.ErrorCodes));
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error! - Message: {string.Format(ex.Message, " - ", ex.StackTrace)}", ex);

                return GetInternalServerErrorObjectResult(new List<string> { $"Unexpected error! - Message: {string.Format(ex.Message, " - ", ex.StackTrace)}" });
            }
        }

        public async Task<ObjectResult> ReturnWhitoutSummaryAsync<TRequest, TResult, T>(Func<TRequest, Task<TResult>> action, TRequest request, int successStatusCode = (int)HttpStatusCode.OK, bool ignoreMessageCodesFactory = false)
            where TRequest : BaseDTO.Request
            where TResult : BaseDTO.Response<T>
        {
            try
            {
                var validation = _validationHelper.Validate(request, ignoreMessageCodesFactory);

                if (validation.IsValid)
                {
                    var response = await action(request);

                    switch ((HttpStatusCode)response.StatusCode)
                    {
                        case HttpStatusCode statusCode when statusCode.IsSuccessHttpStatusCode():
                            return new OkObjectResult(response.Content) { StatusCode = successStatusCode };
                        case HttpStatusCode.Unauthorized:
                            return new UnauthorizedObjectResult(response);
                        case HttpStatusCode.Forbidden:
                            return new ForbidenObjectResult(response);
                        case HttpStatusCode.NotFound:
                            return new NotFoundObjectResult(response);
                        case HttpStatusCode.NoContent:
                            return new NoContentObjectResult(response);
                        case HttpStatusCode.BadRequest:
                            return new BadRequestObjectResult(response);
                        case HttpStatusCode.InternalServerError:
                            return new InternalServerErrorObjectResult(response);
                        default:
                            return GetBadRequestObjectResult(new List<string> { response.Message });
                    }
                }
                else if (validation.ErrorCodes.Contains(MessageCodesFactory.UNEXPECTED_ERROR))
                {
                    return GetInternalServerErrorObjectResult(new InternalServerErrorResultDto(validation.ErrorCodes));
                }
                else
                {
                    return GetBadRequestObjectResult(new BadRequestObjectResultDto(validation.ErrorCodes));
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error! - Message: {string.Format(ex.Message, " - ", ex.StackTrace)}", ex);

                return GetInternalServerErrorObjectResult(new List<string> { $"Unexpected error! - Message: {string.Format(ex.Message, " - ", ex.StackTrace)}" });
            }
        }

        public async Task<IActionResult> ReturnFileAsync<TRequest, TResult, T>(Func<TRequest, Task<TResult>> action, TRequest request, string contentType, int successStatusCode = (int)HttpStatusCode.OK, bool ignoreMessageCodesFactory = false)
            where TRequest : BaseDTO.Request
            where TResult : BaseDTO.Response<byte[]>
        {
            try
            {
                var validation = _validationHelper.Validate(request, ignoreMessageCodesFactory);

                if (validation.IsValid)
                {
                    var response = await action(request);

                    switch ((HttpStatusCode)response.StatusCode)
                    {
                        case HttpStatusCode statusCode when statusCode.IsSuccessHttpStatusCode():
                            return new FileContentResult(response.Content, contentType);
                        case HttpStatusCode.Unauthorized:
                            return new UnauthorizedObjectResult(response);
                        case HttpStatusCode.Forbidden:
                            return new ForbidenObjectResult(response);
                        case HttpStatusCode.NotFound:
                            return new NotFoundObjectResult(response);
                        case HttpStatusCode.NoContent:
                            return new NoContentObjectResult(response);
                        case HttpStatusCode.BadRequest:
                            return new BadRequestObjectResult(response);
                        default:
                            return GetBadRequestObjectResult(new List<string> { response.Message });
                    }
                }
                else if (validation.ErrorCodes.Contains(MessageCodesFactory.UNEXPECTED_ERROR))
                {
                    return GetInternalServerErrorObjectResult(new InternalServerErrorResultDto(validation.ErrorCodes));
                }
                else
                {
                    return GetBadRequestObjectResult(new BadRequestObjectResultDto(validation.ErrorCodes));
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error! - Message: {string.Format(ex.Message, " - ", ex.StackTrace)}", ex);

                return GetInternalServerErrorObjectResult(new List<string> { $"Unexpected error! - Message: {string.Format(ex.Message, " - ", ex.StackTrace)}" });
            }
        }

        private InternalServerErrorObjectResult GetInternalServerErrorObjectResult(List<string> errorCodes)
            => new InternalServerErrorObjectResult(errorCodes);

        private InternalServerErrorObjectResult GetInternalServerErrorObjectResult(InternalServerErrorResultDto errors)
            => new InternalServerErrorObjectResult(errors);

        private BadRequestObjectResult GetBadRequestObjectResult(List<string> errorCodes)
            => new BadRequestObjectResult(errorCodes);

        private BadRequestObjectResult GetBadRequestObjectResult(BadRequestObjectResultDto errors)
            => new BadRequestObjectResult(errors);

        #endregion
    }
}
