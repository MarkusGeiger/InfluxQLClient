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
  
  public async Task<List<string?>?> SelectLastValue(string database, string measurement, string last, string tagKey, string tagValue)
  {
    var result = await QueryFromInflux($"SELECT last({last}) FROM {measurement} WHERE {tagKey}='{tagValue}'", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string?>?> SelectFirstValue(string database, string measurement, string first, string tagKey, string tagValue)
  {
    var result = await QueryFromInflux($"SELECT first({first}) FROM {measurement} WHERE {tagKey}='{tagValue}'", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }
}