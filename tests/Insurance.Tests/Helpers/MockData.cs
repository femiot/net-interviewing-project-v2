using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;

namespace Insurance.Tests.Helpers
{
    public static class MockData
    {
        public static IFormFile GetTestFormFile(this string fileName)
        {
            var content = "ProductTypeId| SurchargeRate" + Environment.NewLine +
                "32 | 2300,123" + Environment.NewLine +
                "33 | 500,45" + Environment.NewLine +
                "124 | 1200" + Environment.NewLine +
                "21 | 1000,34";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "data", fileName);
        }
    }
}
