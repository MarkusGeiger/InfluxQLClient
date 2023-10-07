using InfluxQl.Client;

namespace InfluxQl.Tests;

[TestClass]
public class CreateClientTests
{
    [TestMethod]
    public void CreateEmptyClient()
    {
      var client = InfluxQlClient.GetClient();
      Assert.IsNotNull(client);
    }
    
    [TestMethod]
    public void CreateClientFromHost()
    {
      var client = InfluxQlClient.GetClient("localhost");
      Assert.IsNotNull(client);
    }

    [TestMethod]
    public void CreateClientFromHostAndPort()
    {
      var client = InfluxQlClient.GetClient("localhost", 8086);
      Assert.IsNotNull(client);
    }

    [TestMethod]
    public void CreateClientFromHostPortAndSchema()
    {
      var client = InfluxQlClient.GetClient("http", "localhost", 8086);
      Assert.IsNotNull(client);
    }

    [TestMethod]
    public void CreateClientFromAllParameters()
    {
      var client = InfluxQlClient.GetClient("http", "localhost", 8086, "/");
      Assert.IsNotNull(client);
    }

    [TestMethod]
    public void CreateClientFromUri()
    {
      var uri = new Uri("http://localhost:8086");
      var client = InfluxQlClient.GetClient(uri);
      Assert.IsNotNull(client);
    }
}