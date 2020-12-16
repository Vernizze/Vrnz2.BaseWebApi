using FluentValidation;

namespace Vrnz2.BaseWebApi.Validations
{
    public class PingRequestValidator
        : AbstractValidator<BaseContracts.DTOs.Ping.Request>
    {
        #region Constructors

        public PingRequestValidator() { }

        #endregion
    }
}
