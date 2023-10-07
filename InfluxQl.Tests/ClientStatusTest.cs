using InfluxQl.Client;

namespace InfluxQl.Tests;

[TestClass]
public class ClientStatusTest
{
  [TestMethod]
  public async Task CheckStatus()
  {
    var client = InfluxQlClient.GetClient("192.168.2.128");
    Assert.IsNotNull(client);
    var pingResult = await client.Status.Ping();
    Assert.IsTrue(pingResult);
  }
}
