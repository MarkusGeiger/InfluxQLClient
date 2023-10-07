using System.Net.NetworkInformation;

namespace InfluxQl.Client;

public class InfluxStatus
{
  private InfluxQlClient client;

  public InfluxStatus(InfluxQlClient influxQlClient)
  {
    this.client = influxQlClient;
  }

  public async Task<bool> Ping()
  {
    var pingResponse = await client.Get("/ping");
    return pingResponse != null && pingResponse == string.Empty;
  }
}