using System.Text;
using System.Text.Json;

namespace InfluxQl.GrafanaProxy;

public class GrafanaClient
{
  private readonly HttpClient client;

  public GrafanaClient(string url)
  {
    client = new HttpClient();
    client.BaseAddress = new Uri(url);
    
    var byteArray = Encoding.ASCII.GetBytes("admin:admin");
    var base64Value = Convert.ToBase64String(byteArray);
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Value);
  }

  public async Task<string> GetInfluxDatabaseId(string name)
  {
    var response = await Get($"/api/datasources/name/{name}");
    var influxDatabase = JsonSerializer.Deserialize<GrafanaDatasource>(response);
    return influxDatabase.uid;
  }

  internal async Task<string?> Get(string path)
  {
    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.NotImplemented);
    try
    {
      response = await client.GetAsync(path);
      Console.WriteLine($"HTTP GET: status: [{response.StatusCode}], path: '{path}'");

      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadAsStringAsync();
      }
      else
      {
        Console.WriteLine(response);
      }
    }
    catch (System.Exception exc)
    {
      System.Console.WriteLine(exc);
      throw;
    }

    return null;
  }
}

public class GrafanaDatasource
{
  public int id { get; set; }
  public string uid { get; set; }
}