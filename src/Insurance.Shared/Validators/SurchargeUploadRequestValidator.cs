using FluentValidation;
using Insurance.Shared.Payload.Requests;
using Microsoft.AspNetCore.Http;

namespace Insurance.Shared.Validators
{
    public class SurchargeUploadRequestValidator : AbstractValidator<SurchargeUploadRequest>
    {
        public SurchargeUploadRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Please provide User Id");
            RuleFor(x => x.SurchargeFile).Must(x => BeValidFile(x)).WithMessage("Please upload a valid csv file (1 GB size max)");
        }

        public bool BeValidFile(IFormFile surchargeFile)
        {
            var fileExtension = Path.GetExtension(surchargeFile.FileName);
            if (fileExtension != ".csv")
                return false;

            if (surchargeFile.Length > 1048576000)
                return false;

            return true;
        }
    }
}
