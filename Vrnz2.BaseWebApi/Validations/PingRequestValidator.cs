using FluentValidation;
using Vrnz2.BaseContracts.DTOs;

namespace Vrnz2.BaseWebApi.Validations
{
    public class PingRequestValidator
        : AbstractValidator<Ping.Request>
    {
        #region Constructors

        public PingRequestValidator() { }

        #endregion
    }
}
