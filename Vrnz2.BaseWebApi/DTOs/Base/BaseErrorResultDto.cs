using System.Collections.Generic;
using Vrnz2.BaseContracts.DTOs.Base;

namespace Vrnz2.BaseWebApi.DTOs.Base
{
    public abstract class BaseErrorResultDto
        : BaseDTO.Response<List<string>>
    {
        public BaseErrorResultDto(List<string> errors)
            => Content = errors;
    }
}
