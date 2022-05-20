using FluentValidation;
using Insurance.Shared.Payload.Requests;

namespace Insurance.Shared.Validators
{
    public class SurchargeUploadRequestValidator : AbstractValidator<SurchargeUploadRequest>
    {
        public SurchargeUploadRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("Please provide User Id");
            RuleFor(x => x).NotNull().Must(x => BeValidFile(x)).OverridePropertyName(x => x.SurchargeFile).WithMessage("Please upload a valid csv file (1 GB size max)");
        }

        public bool BeValidFile(SurchargeUploadRequest surchargeUploadRequest)
        {
            var fileExtension = Path.GetExtension(surchargeUploadRequest.SurchargeFile.FileName);

            if (fileExtension != ".csv")
                return false;

            if (surchargeUploadRequest.SurchargeFile.Length <= 0
                    || surchargeUploadRequest.SurchargeFile.Length > 1048576000)
                return false;

            if (surchargeUploadRequest.BuildSurchageRateFromFile() == null)
                return false;

            return true;
        }
    }
}
