using InfluxQl.Client;

namespace InfluxQl.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
      var client = new InfluxQlClient();
      Assert.IsNotNull(client);
    }
}