// See https://aka.ms/new-console-template for more information
using InfluxQl.Client;

Console.WriteLine("Hello, World!");

var client = InfluxQlClient.GetClient("192.168.2.128");
var databases = await client.Metadata.ShowDatabases();
Console.WriteLine($"SHOW DATABASES result:\n[{string.Join(", ", databases)}]");

foreach (var database in databases)
{
  var measurements = await client.Metadata.ShowMeasurements(database);
  Console.WriteLine($"SHOW MEASUREMENTS on database: '{database}', result:\n\t[{string.Join(", ", measurements)}]");

  foreach (var measurement in measurements)
  {
    var fieldKeys = await client.Metadata.ShowFieldKeys(database, measurement);
    Console.WriteLine($"\t\tSHOW FIELD KEYS from measurement: '{measurement}' on database: '{database}', result:\n\t\t[{string.Join(", ", fieldKeys)}]");

    var tagKeys = await client.Metadata.ShowTagKeys(database, measurement);
    Console.WriteLine($"\t\tSHOW TAG KEYS from measurement: '{measurement}' on database: '{database}', result:\n\t\t[{string.Join(", ", tagKeys)}]");
    foreach (var tagKey in tagKeys)
    {
      var tagValues = await client.Metadata.ShowTagValues(database, measurement, tagKey);
      Console.WriteLine($"\t\t\tSHOW TAG VALUES for key={tagKey} from measurement: '{measurement}' on database: '{database}', result:\n\t\t\t[{string.Join(", ", tagValues)}]");
    }
  }

  Console.WriteLine("####### ALL TAGS ########");
  
  var allTagKeys = await client.Metadata.ShowTagKeys(database);
  Console.WriteLine($"SHOW TAG KEYS on database: '{database}', result:\n[{string.Join(", ", allTagKeys)}]");
  foreach (var tagKey in allTagKeys)
  {
    var tagValues = await client.Metadata.ShowTagValues(database, tagKey);
    Console.WriteLine($"\tSHOW TAG VALUES for key={tagKey} on database: '{database}', result:\n\t[{string.Join(", ", tagValues)}]");
  }

  Console.WriteLine("#######################");
}