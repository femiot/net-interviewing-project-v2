using CsvHelper;
using Insurance.Shared.DTOs;
using Insurance.Shared.Helper;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Insurance.Shared.Payload.Requests
{
    public class SurchargeUploadRequest
    {
        public Guid UserId { get; set; }
        public IFormFile SurchargeFile { get; set; } = null!;

        public IEnumerable<SurchargeRateDto>? BuildSurchageRateFromFile()
        {
            if (SurchargeFile != null && SurchargeFile.FileName.Contains(".csv"))
                using (var memory = new MemoryStream())
                {
                    SurchargeFile.CopyTo(memory);

                    using (var reader = new StreamReader(memory))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<SurchargeRateDto>();
                        return records;
                    }
                }
            return null;
        }
    }
}
