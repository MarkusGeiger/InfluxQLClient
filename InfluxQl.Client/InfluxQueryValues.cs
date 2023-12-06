using InfluxQl.Client;

public class InfluxQueryValues : InfluxQueryBase
{
  public InfluxQueryValues(InfluxQlClient influxQlClient) : base(influxQlClient)
  {
  }

  public async Task<List<InfluxSeries>> SelectLastValueAsTable(string database, string measurement)
  {
    var result = await base.QueryAsTableFromInflux($"SELECT * FROM {measurement} LIMIT 1", database);
    return result;
  }
  
  public async Task<List<string?>?> SelectLastValue(string database, string retentionPolicy, string measurement, string last, string tagKey, string tagValue, DateTime oldestValue)
  {
    var result = await QueryFromInflux($"""SELECT last("{last}") FROM "{retentionPolicy}"."{measurement}" WHERE {tagKey}='{tagValue}' AND time < '{oldestValue:O}'""", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    if (result?.InfluxResults == null || !result.InfluxResults.Any() || !result.InfluxResults.First().Series.Any()) return new List<string?>();
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string?>?> SelectFirstValue(string database, string retentionPolicy, string measurement, string first, string tagKey, string tagValue, DateTime newestValue)
  {
    var result = await QueryFromInflux($"""SELECT first("{first}") FROM "{retentionPolicy}"."{measurement}" WHERE {tagKey}='{tagValue}' AND time > '{newestValue:O}'""", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    if (result?.InfluxResults == null || !result.InfluxResults.Any() || !result.InfluxResults.First().Series.Any()) return new List<string?>();
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }
}