using System.Text.Json;

namespace InfluxQl.Client;

public abstract class InfluxQueryBase
{
  protected InfluxQlClient client;

  public InfluxQueryBase(InfluxQlClient influxQlClient)
  {
    this.client = influxQlClient;
  }

  protected virtual async Task<string> QueryStringFromInflux(string query, string database = "")
  {
    if (query.ToUpperInvariant().StartsWith("SELECT") || query.ToUpperInvariant().StartsWith("SHOW"))
    {
      var queryParameter = $"q={query}";
      var parameters = new List<string>();
      if (!string.IsNullOrWhiteSpace(database))
      {
        parameters.Add($"db={database}");
      }
      parameters.Add(queryParameter);
      var response = await client.Get($"query?{string.Join("&", parameters)}");
      //Console.WriteLine($"QUERY raw result: {response}, query request: '{query}'");
      return response ?? throw new NullReferenceException("Query response was null.");
    }
    else
    {
      throw new NotImplementedException("POST has to be implemented for all queries except SELECT and SHOW");
    }
  }

  protected virtual async Task<InfluxResponse> QueryFromInflux(string query, string database = "")
  {
    var responseText = await QueryStringFromInflux(query, database);
    Console.WriteLine(responseText);
    var response = JsonSerializer.Deserialize<InfluxResponse>(responseText);

    return response ?? throw new NullReferenceException("Could not parse response to json.");
  }

  protected virtual async Task<List<InfluxSeries>> QueryAsTableFromInflux(string query, string database = "")
  {
    var result = await QueryFromInflux(query, database);
    return result.GetAsTable();
  }
}