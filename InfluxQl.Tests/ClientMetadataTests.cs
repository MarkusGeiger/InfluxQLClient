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
  }

  [TestMethod]
  public async Task GetMeasurements()
  {
    var client = InfluxQlClient.GetClient("192.168.2.128");
    Assert.IsNotNull(client);
    var databases = await client.Metadata.ShowDatabases();
    Assert.IsNotNull(databases);
    Assert.IsTrue(databases.Any());
    
    var measurements = await client.Metadata.ShowMeasurements(databases.First());
    Assert.IsNotNull(measurements);
    Assert.IsTrue(measurements.Any());
  }
  
  [TestMethod]
  public async Task GetFieldKeys()
  {
    var client = InfluxQlClient.GetClient("192.168.2.128");
    Assert.IsNotNull(client);
    var databases = await client.Metadata.ShowDatabases();
    Assert.IsNotNull(databases);
    Assert.IsTrue(databases.Any());
    
    var keys = await client.Metadata.ShowFieldKeys(databases.First());
    Assert.IsNotNull(keys);
    Assert.IsTrue(keys.Any());
  }
  
  [TestMethod]
  public async Task GetTagKeys()
  {
    var client = InfluxQlClient.GetClient("192.168.2.128");
    Assert.IsNotNull(client);
    var databases = await client.Metadata.ShowDatabases();
    Assert.IsNotNull(databases);
    Assert.IsTrue(databases.Any());
    
    var tags = await client.Metadata.ShowTagKeys(databases.First());
    Assert.IsNotNull(tags);
    Assert.IsTrue(tags.Any());
  }

  [TestMethod]
  public async Task GetTagValues()
  {
    var client = InfluxQlClient.GetClient("192.168.2.128");
    Assert.IsNotNull(client);
    var databases = await client.Metadata.ShowDatabases();
    Assert.IsNotNull(databases);
    Assert.IsTrue(databases.Any());
    
    var tags = await client.Metadata.ShowTagKeys(databases.First());
    Assert.IsNotNull(tags);
    Assert.IsTrue(tags.Any());

    var tagValues = await client.Metadata.ShowTagValues(databases.First(), tags.First());
    Assert.IsNotNull(tagValues);
    Assert.IsTrue(tagValues.Any());
  }
}
