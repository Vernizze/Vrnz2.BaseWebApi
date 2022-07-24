using System.Collections.Generic;
using Vrnz2.BaseWebApi.DTOs.Base;

namespace Vrnz2.BaseWebApi.DTOs
{
    public class BadRequestObjectResultDto
        : BaseErrorResultDto
    {
        public BadRequestObjectResultDto(List<string> errors)
            : base(errors)
        {
        }
    }
}
