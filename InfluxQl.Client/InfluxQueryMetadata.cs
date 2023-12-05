


namespace InfluxQl.Client;

public class InfluxQueryMetadata : InfluxQueryBase
{
  public InfluxQueryMetadata(InfluxQlClient influxQlClient) : base(influxQlClient) { }

  public async Task<List<string>> ShowDatabases()
  {
    var result = await QueryFromInflux("SHOW DATABASES");
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowMeasurements(string database)
  {
    var result = await QueryFromInflux("SHOW MEASUREMENTS", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }
  
  public async Task<List<string>> ShowMeasurements(string database, string tagKey, string tagValue)
  {
    var result = await QueryFromInflux($"SHOW MEASUREMENTS WHERE {tagKey}='{tagValue}'", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowFieldKeys(string database)
  {
    var result = await QueryFromInflux($"SHOW FIELD KEYS", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowFieldKeys(string database, string measurement)
  {
    var result = await QueryFromInflux($"SHOW FIELD KEYS FROM \"{measurement}\"", database);
    System.Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowTagKeys(string database)
  {
    var result = await QueryFromInflux($"SHOW TAG KEYS", database);
    Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowTagKeys(string database, string measurement)
  {
    var result = await QueryFromInflux($"SHOW TAG KEYS FROM \"{measurement}\"", database);
    Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<string>> ShowTagValues(string database, string tagKey)
  {
    var result = await QueryFromInflux($"SHOW TAG VALUES WITH key=\"{tagKey}\"", database);
    Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    // 1 Result, 12 Series
    // TODO: These series are all tag values for the given tag key for each measurement in the database
    var series = result.InfluxResults.First().Series;
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<InfluxSeries>> ShowTagValuesAsTables (string database, string tagKey)
  {
    var result = await QueryFromInflux($"SHOW TAG VALUES WITH key=\"{tagKey}\"", database);
    //Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    // 1 Result, 12 Series
    // TODO: These series are all tag values for the given tag key for each measurement in the database
    return result.GetAsTable();
  }

  private string printList(List<string> columns) => $"[{columns.Count}]{{{string.Join(", ", columns.Take(2))}, ...}}";

  public async Task<List<string>> ShowTagValues(string database, string measurement, string tagKey)
  {
    var result = await QueryFromInflux($"SHOW TAG VALUES FROM \"{measurement}\" WITH key=\"{tagKey}\"", database);
    //Console.WriteLine($"Number of results: {result.InfluxResults.Count}, Number of Series: {result.InfluxResults.Sum(res => res.Series.Count)}");
    
    var resultList = result.InfluxResults.First().Series.First().Table.SelectMany(valueRow => valueRow.Select(val => val.ToString())).ToList();
    return resultList;
  }

  public async Task<List<InfluxSeries>> ShowTagValuesAsTables(string database, string measurement, string tagKey)
  {
    return await QueryAsTableFromInflux($"SHOW TAG VALUES FROM \"{measurement}\" WITH key=\"{tagKey}\"", database);
  }

  public async Task<List<InfluxSeries>> ShowSeriesAsTables(string database, string measurement)
  {
    return await QueryAsTableFromInflux($"SHOW SERIES FROM \"{measurement}\"", database);
  }

  public async Task<List<InfluxSeries>> ShowMeasurementsAsTables(string database)
  {
    return await QueryAsTableFromInflux("SHOW MEASUREMENTS", database);
  }
}
