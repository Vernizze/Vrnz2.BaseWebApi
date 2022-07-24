using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using Vrnz2.BaseContracts.DTOs.Base;

namespace Vrnz2.BaseWebApi.Helpers
{
    public interface IControllerHelper
    {
        #region Methods

        Task<ObjectResult> ReturnAsync<TRequest, TResult, T>(Func<TRequest, Task<TResult>> action, TRequest request, int successStatusCode = (int)HttpStatusCode.OK, bool ignoreMessageCodesFactory = false)
            where TRequest : BaseDTO.Request
            where TResult : BaseDTO.Response<T>;

        Task<ObjectResult> ReturnWhitoutSummaryAsync<TRequest, TResult, T>(Func<TRequest, Task<TResult>> action, TRequest request, int successStatusCode = (int)HttpStatusCode.OK, bool ignoreMessageCodesFactory = false)
            where TRequest : BaseDTO.Request
            where TResult : BaseDTO.Response<T>;

        Task<IActionResult> ReturnFileAsync<TRequest, TResult, T>(Func<TRequest, Task<TResult>> action, TRequest request, string contentType, int successStatusCode = (int)HttpStatusCode.OK, bool ignoreMessageCodesFactory = false)
            where TRequest : BaseDTO.Request
            where TResult : BaseDTO.Response<byte[]>;

        #endregion
    }
}
