


namespace InfluxQl.Client;

public class InfluxQueryMetadata : InfluxQueryBase
{
  public InfluxQueryMetadata(InfluxQlClient influxQlClient) : base(influxQlClient) { }

  public async Task<List<string>> ShowDatabases()
  {
    var result = await QueryFromInflux("SHOW DATABASES");
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowMeasurements(string database)
  {
    var result = await QueryFromInflux("SHOW MEASUREMENTS", database);
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowFieldKeys(string database)
  {
    var result = await QueryFromInflux($"SHOW FIELD KEYS", database);
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowFieldKeys(string database, string measurement)
  {
    var result = await QueryFromInflux($"SHOW FIELD KEYS FROM \"{measurement}\"", database);
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowTagKeys(string database)
  {
    var result = await QueryFromInflux($"SHOW TAG KEYS", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowTagKeys(string database, string measurement)
  {
    var result = await QueryFromInflux($"SHOW TAG KEYS FROM \"{measurement}\"", database);
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowTagValues(string database, string tagKey)
  {
    var result = await QueryFromInflux($"SHOW TAG VALUES WITH key=\"{tagKey}\"", database);
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowTagValues(string database, string measurement, string tagKey)
  {
    var result = await QueryFromInflux($"SHOW TAG VALUES FROM \"{measurement}\" WITH key=\"{tagKey}\"", database);
    var resultList = result.InfluxResults.First().Series.First().Values.SelectMany(valueRow => valueRow.Select(val => val)).ToList();
    return resultList;
  }
}
