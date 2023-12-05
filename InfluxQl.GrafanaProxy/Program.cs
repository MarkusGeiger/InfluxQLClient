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
    foreach (List<object> row in series.Table)
    {
      var rowString = row.Select(r => r.ToString()).ToList();
      if (rowString.Count == 2 && rowString[0] == tagKey && !string.IsNullOrWhiteSpace(rowString[1]))
      {
        uniqueTagValues.Add(rowString[1]);
      }
    }
  }

  AnsiConsole.WriteLine(string.Join("; ", uniqueTagValues));


  foreach (var tagValue in uniqueTagValues)
  {
    results.Add(tagValue, new ValueItem { Name = tagValue });
    var measurements = await client.Metadata.ShowMeasurements(database, tagKey, tagValue);
    //AnsiConsole.WriteLine(string.Join("; ", measurements));
    foreach (var measurement in measurements)
    {
      var fieldKeys = await client.Metadata.ShowFieldKeys(database, measurement);
      var firstValueRow =
        await client.Values.SelectFirstValue(database, measurement, fieldKeys.First(), tagKey, tagValue);
      //AnsiConsole.WriteLine($"First: {string.Join("; ", firstValueRow)}");
      if (firstValueRow.Count == 2 && DateTime.TryParse(firstValueRow[0], out var firstTimestamp))
      {
        AnsiConsole.MarkupLineInterpolated($"[green]'{measurement}' ({tagValue}): '{firstTimestamp}'[/]");
        if (results.TryGetValue(tagValue, out var currentItem))
        {
          currentItem.UpdateOldestValueIfOlder(firstTimestamp);
        }
        else
        {
          results.Add(tagValue, new ValueItem { Name = tagValue, OldestValue = firstTimestamp });
        }
      }

      var lastValueRow =
        await client.Values.SelectLastValue(database, measurement, fieldKeys.First(), tagKey, tagValue);
      //AnsiConsole.WriteLine($"First: {string.Join("; ", firstValueRow)}");
      if (lastValueRow.Count == 2 && DateTime.TryParse(lastValueRow[0], out var lastTimestamp))
      {
        AnsiConsole.MarkupLineInterpolated($"[green]'{measurement}' ({tagValue}): '{lastTimestamp}'[/]");
        if (results.TryGetValue(tagValue, out var currentItem))
        {
          currentItem.UpdateNewestValueIfNewer(lastTimestamp);
        }
        else
        {
          results.Add(tagValue, new ValueItem { Name = tagValue, NewestValue = lastTimestamp });
        }
      }
    }
  }
}

foreach (var value in results.Values)
{
  AnsiConsole.MarkupLineInterpolated($"[green]'{value}'[/]");
}

public class ValueItem
{
  public string Name { get; set; }
  public DateTime OldestValue { get; set; }
  public DateTime NewestValue { get; set; }

  public void UpdateOldestValueIfOlder(DateTime newValue)
  {
    AnsiConsole.MarkupLine($"?? [red]{newValue} < {OldestValue}[/] ??");
    if (newValue < OldestValue || NewestValue == DateTime.MinValue) OldestValue = newValue;
  }
  
  public void UpdateNewestValueIfNewer(DateTime newValue)
  {
    AnsiConsole.MarkupLine($"?? [red]{newValue} > {NewestValue}[/] ??");
    if (newValue > NewestValue || NewestValue == DateTime.MinValue) NewestValue = newValue;
  }

  public override string ToString()
  {
    return $"{Name} ({OldestValue} to {NewestValue})";
  }
}