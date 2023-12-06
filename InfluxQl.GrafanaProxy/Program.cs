// See https://aka.ms/new-console-template for more information

using System.Text;
using InfluxQl.Client;
using InfluxQl.GrafanaProxy;
using Spectre.Console;

Console.WriteLine("Hello, World!");
//
// var testClient = new HttpClient();
// testClient.BaseAddress = new Uri("http://192.168.2.128:3003/api/datasources/proxy/uid/b2456aa2-d0bf-4335-a976-1495c636c0ab/");
//
// var byteArray = Encoding.ASCII.GetBytes("admin:admin");
// var base64Value = Convert.ToBase64String(byteArray);
// testClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Value);
//
// var response = await testClient.GetAsync("query?q=SHOW DATABASES");
//
//
//
// var clientDirect = InfluxQlClient.GetClient("192.168.2.128");
// var databasesDirect = await clientDirect.Metadata.ShowDatabases();
// Console.WriteLine($"SHOW DATABASES result:\n[{string.Join(", ", databasesDirect)}]");


var grafana = new GrafanaClient("http://192.168.2.128:3003");
var id = await grafana.GetInfluxDatabaseId("InfluxDB");

// http://192.168.2.128:3003/api/datasources/proxy/uid/b2456aa2-d0bf-4335-a976-1495c636c0ab/query?db=telegraf&q=SHOW%20DATABASES&epoch=ms
// http://192.168.2.128:3003/api/datasources/proxy/uid/b2456aa2-d0bf-4335-a976-1495c636c0ab/query?&q=SHOW DATABASES

var client = InfluxQlClient.GetClient("http","192.168.2.128", 3003, $"/api/datasources/proxy/uid/{id}/");
client.AddGrafanaAuth("admin:admin");
var databases = await client.Metadata.ShowDatabases();
Console.WriteLine($"SHOW DATABASES result:\n[{string.Join(", ", databases)}]");


// http://192.168.2.128:3003/api/datasources/proxy/uid/b2456aa2-d0bf-4335-a976-1495c636c0ab
// /query?db=telegraf&q=SELECT%20mean(%22used%22)%20FROM%20%22mem%22%20WHERE%20time%20%3E%3D%20now()%20-%206h%20and%20time%20%3C%3D%20now()%20GROUP%20BY%20time(15s)%20fill(null)&epoch=ms

var tagKey = "host";

var results = new Dictionary<string, ValueItem>();
foreach (var database in databases)
{
  var resultTables = await client.Metadata.ShowTagValuesAsTables(database, tagKey);
// foreach(var result in resultTables)
// {
//   Helpers.PrintTable(result.Name, result.Columns, result.Table);
// }

  HashSet<string> uniqueTagValues = new HashSet<string>();
  foreach (InfluxSeries series in resultTables)
  {
    foreach (var row in series.Table)
    {
      var rowString = row.Select(r => r.ToString()).ToList();
      if (rowString != null && rowString.Count == 2 && rowString[0] == tagKey && !string.IsNullOrWhiteSpace(rowString[1]))
      {
        uniqueTagValues.Add(rowString[1]);
      }
    }
  }

  AnsiConsole.WriteLine(string.Join("; ", uniqueTagValues));

  foreach (var tagValue in uniqueTagValues)
  {
    if (!results.TryGetValue(tagValue, out var valueItem))
    {
      valueItem = new ValueItem(tagValue);
      valueItem.Databases.Add(database);
      results.Add(tagValue, valueItem);
    }
    var measurements = await client.Metadata.ShowMeasurements(database, tagKey, tagValue);
    foreach (var measurement in measurements)
    {
      valueItem.Measurements.Add(measurement);
      var fieldKeys = await client.Metadata.ShowFieldKeys(database, measurement);
      if (!fieldKeys.Any()) continue;

      foreach (var fieldKey in fieldKeys)
      {
        var retentionPolicy = "autogen";
        var firstValueRow = await client.Values.SelectFirstValue(database, retentionPolicy, measurement, fieldKey, tagKey, tagValue, valueItem.OldestValue);
        if (firstValueRow is { Count: 2 } && DateTime.TryParse(firstValueRow[0], out var firstTimestamp))
        {
          AnsiConsole.MarkupLineInterpolated($"[green]'{measurement}' ({tagValue}): '{firstTimestamp}'[/]");
          valueItem.UpdateOldestValueIfOlder(firstTimestamp);
        }

        var lastValueRow = await client.Values.SelectLastValue(database, retentionPolicy, measurement, fieldKey, tagKey, tagValue, valueItem.NewestValue);
        if (lastValueRow is { Count: 2 } && DateTime.TryParse(lastValueRow[0], out var lastTimestamp))
        {
          AnsiConsole.MarkupLineInterpolated($"[green]'{measurement}' ({tagValue}): '{lastTimestamp}'[/]");
          valueItem.UpdateNewestValueIfNewer(lastTimestamp);
        }
      }
    }

    results[tagValue] = valueItem;
  }
}

foreach (var value in results.Values)
{
  AnsiConsole.MarkupLineInterpolated($"[green]'{value.ToString().EscapeMarkup()}'[/]");
}

public class ValueItem
{
  public string Name { get; }
  public DateTime OldestValue { get; set; } = DateTime.UtcNow;
  public DateTime NewestValue { get; set; } = DateTime.MinValue;

  public List<string> Databases { get; } = new ();
  public List<string> Measurements { get; } = new ();

  public ValueItem(string name) => Name = name;

  public void UpdateOldestValueIfOlder(DateTime newValue)
  {
    AnsiConsole.MarkupLine($"?? [red]{newValue} < {OldestValue}[/] ??");
    if (newValue > DateTime.UtcNow.AddDays(1)) return; // new Value is in the future
    if (newValue < DateTime.UtcNow.AddYears(-10)) return; // This value is definitely too old
    
    if (newValue < OldestValue) OldestValue = newValue;
  }
  
  public void UpdateNewestValueIfNewer(DateTime newValue)
  {
    AnsiConsole.MarkupLine($"?? [red]{newValue} > {NewestValue}[/] ??");
    if (newValue > DateTime.UtcNow.AddDays(1)) return; // new Value is in the future
    if (newValue < DateTime.UtcNow.AddYears(-10)) return; // This value is definitely too old
    
    if (newValue > NewestValue) NewestValue = newValue;
  }

  private static string GetUpToFiveValues(List<string> values)
  {
    var builder = new StringBuilder();
    builder.Append('[');
    if (!values.Any())
    {
      builder.Append("<none>");
    }
    else if (values.Count > 5)
    {
      builder.Append(string.Join(',', values.Take(5)));
      builder.Append(",...");
    }
    else
    {
      builder.Append(string.Join(',', values));
    }

    builder.Append(']');
    return builder.ToString();
  }
  
  public override string ToString()
  {
    return $"{Name};{OldestValue};{NewestValue};{GetUpToFiveValues(Measurements)};{GetUpToFiveValues(Databases)}";
  }
}