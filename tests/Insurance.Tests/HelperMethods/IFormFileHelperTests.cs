using Insurance.Shared.Helper;
using Insurance.Tests.Helpers;
using Xunit;

namespace Insurance.Tests.HelperMethods
{
    public class IFormFileHelperTests
    {
        [Fact]
        public void ReadAsList_Given_A_Valid_File_Should_Return_List_Of_Strings()
        {
            var fileName = "test.csv";

            var file = fileName.GetTestFormFile();

            var result = FormFileHelper.ReadAsList(file);

            Assert.True(result.Count == 5);
        }

        [Fact]
        public void ReadAsList_Given_An_Invalid_Should_Return_Empty_List()
        {
            var result = FormFileHelper.ReadAsList(null);

            Assert.True(result.Count == 0);
        }
    }
}
