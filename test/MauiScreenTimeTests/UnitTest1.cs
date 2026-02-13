using MauiScreenTime.ViewModels;

namespace MauiScreenTimeTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            TestingBinding tb = new TestingBinding();
            Assert.True(tb.GetIsAccessible);
        }
    }
}
