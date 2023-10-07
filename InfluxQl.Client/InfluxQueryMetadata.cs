namespace InfluxQl.Client;

public class InfluxQueryMetadata : InfluxQueryBase
{
  public InfluxQueryMetadata(InfluxQlClient influxQlClient) : base(influxQlClient) { }

  public async Task<List<string>> ShowDatabases()
  {
    var result = await QueryFromInflux("SHOW DATABASES");
    var resultList = result.Split(" ");
    return resultList.ToList();
  }
}
