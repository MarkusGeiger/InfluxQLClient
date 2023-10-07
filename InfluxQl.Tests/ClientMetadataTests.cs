using InfluxQl.Client;

namespace InfluxQl.Tests;

[TestClass]
public class ClientMetadataTests
{
  [TestMethod]
  public async Task GetDatabases()
  {
    var client = InfluxQlClient.GetClient("192.168.2.128");
    Assert.IsNotNull(client);
    var databases = await client.Metadata.ShowDatabases();
    Assert.IsNotNull(databases);
    Console.WriteLine(databases);
  }
}
