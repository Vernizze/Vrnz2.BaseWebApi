using System.Collections.Generic;
using Vrnz2.BaseWebApi.DTOs.Base;

namespace Vrnz2.BaseWebApi.DTOs
{
    public class InternalServerErrorResultDto
        : BaseErrorResultDto
    {
        public InternalServerErrorResultDto(List<string> errors)
            : base(errors)
        {
        }
    }
}
