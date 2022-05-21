using Xunit;

namespace Insurance.Tests.Helpers
{
    [CollectionDefinition("Tests collection")]
    public class TestsCollection : ICollectionFixture<TestFixture<TestStartup>>
    {
    }
}
