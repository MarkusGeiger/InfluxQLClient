namespace InfluxQl.Client;

public abstract class InfluxQueryBase
{
  protected InfluxQlClient client;

  public InfluxQueryBase(InfluxQlClient influxQlClient)
  {
    this.client = influxQlClient;
  }

  protected virtual async Task<string> QueryFromInflux(string query, string database = "")
  {
    if (query.ToUpperInvariant().StartsWith("SELECT") || query.ToUpperInvariant().StartsWith("SHOW"))
    {
      var queryParameter = $"q={query}";
      var dbParameter = string.IsNullOrWhiteSpace(database) ? string.Empty : $"db={database}";
      var parameters = new string[]{dbParameter, queryParameter};
      var response = await client.Get($"/query?{string.Join("&", parameters)}");

      return response;
    }
    else
    {
      throw new NotImplementedException("POST has to be implemented for all queries except SELECT and SHOW");
    }
  }
}