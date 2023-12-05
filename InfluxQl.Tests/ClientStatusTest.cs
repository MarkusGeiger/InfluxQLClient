using InfluxQl.Client;

namespace InfluxQl.Tests;

[TestClass]
public class ClientStatusTest
{
  [TestMethod]
  public async Task CheckStatusSuccess()
  {
    var client = InfluxQlClient.GetClient("192.168.2.128");
    Assert.IsNotNull(client);
    var pingResult = await client.Status.Ping();
    Assert.IsTrue(pingResult);
  }

  [TestMethod]
  public async Task CheckStatusFailed()
  {
    await Assert.ThrowsExceptionAsync<HttpRequestException>(async () =>
    {
      var client = InfluxQlClient.GetClient("127.0.0.1");
      Assert.IsNotNull(client);
      var pingResult = await client.Status.Ping();
      Assert.IsFalse(pingResult);
    }, "this should not work. Are you sure there is no local influx?");
  }
}
