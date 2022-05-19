using Microsoft.AspNetCore.Http;

namespace Insurance.Shared.Helper
{
    public static class FormFileHelper
    {
        public static List<string?> ReadAsList(this IFormFile file)
        {
            var result = new List<string?>();
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.Add(reader.ReadLine());
            }
            return result.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }
    }
}
