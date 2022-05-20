using Insurance.Shared.DTOs;
using Microsoft.AspNetCore.Http;

namespace Insurance.Shared.Payload.Requests
{
    public class SurchargeUploadRequest
    {
        public Guid UserId { get; set; }
        public IFormFile SurchargeFile { get; set; } = null!;

        public IEnumerable<SurchargeRateDto>? BuildSurchageRateFromFile()
        {
            var SurchargeRateDtoList = new List<SurchargeRateDto>();
            if (SurchargeFile != null && SurchargeFile.FileName.Contains(".csv"))
            {
                using var reader = new StreamReader(SurchargeFile.OpenReadStream());
                string recordData = reader.ReadToEnd();

                string[] records = recordData.Split(Environment.NewLine);
                foreach (var record in records)
                {
                    var columns = record.Split('|');
                    if (columns.Length == 2)
                    {
                        if (columns[0].Trim() == nameof(SurchargeRateDto.ProductTypeId) && columns[1].Trim() == nameof(SurchargeRateDto.SurchargeRate))
                            continue;

                        if (int.TryParse(columns[0].Trim(), out var productTypeId) && float.TryParse(columns[1].Trim(), out var surchargeRate))
                            SurchargeRateDtoList.Add(new SurchargeRateDto { ProductTypeId = productTypeId, SurchargeRate = surchargeRate });
                        else
                            throw new Exception($"Unable to capture surcharge rates file. Method {nameof(BuildSurchageRateFromFile)}");
                    }
                }

                if (SurchargeRateDtoList.Count > 0)
                    return SurchargeRateDtoList;
            }
            return null;
        }
    }
}
