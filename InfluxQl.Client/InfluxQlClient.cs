using System.Diagnostics.Contracts;

namespace InfluxQl.Client;

public class InfluxQlClient
{
  private Uri uri;
  private HttpClient client;

  private InfluxQlClient()
  {
    System.Console.WriteLine("Instance created!");
    uri = new Uri("http://localhost:8086");
  }

  private InfluxQlClient(Uri uri)
  {
    this.uri = uri;

    this.client = new HttpClient
    {
      BaseAddress = this.uri,
      Timeout = TimeSpan.FromSeconds(5)
    };

    this.Status = new InfluxStatus(this);
    this.Metadata = new InfluxQueryMetadata(this);
  }

  public static InfluxQlClient GetClient(Uri uri)
  {
    var client = new InfluxQlClient(uri);
    return client;
  }

  public static InfluxQlClient GetClient()
  {
    return GetClient(new UriBuilder("http", "localhost", 8086).Uri);
  }

  public static InfluxQlClient GetClient(string host)
  {
    return GetClient(new UriBuilder("http", host, 8086).Uri);
  }

  public static InfluxQlClient GetClient(string host, int port)
  {
    return GetClient(new UriBuilder("http", host, port).Uri);
  }

  public static InfluxQlClient GetClient(string schema, string host, int port)
  {
    return GetClient(new UriBuilder(schema, host, port).Uri);
  }

  public static InfluxQlClient GetClient(string schema, string host, int port, string path)
  {
    return GetClient(new UriBuilder(schema, host, port, path).Uri);
  }

  public InfluxStatus Status { get; }

  public InfluxQueryMetadata Metadata { get; }

  internal async Task<string?> Get(string path)
  {
    HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.NotImplemented);
    try
    {
      response = await client.GetAsync(path);
      Console.WriteLine($"HTTP GET: status: [{response.StatusCode}], path: '{path}'");
      
      if(response.IsSuccessStatusCode)
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
