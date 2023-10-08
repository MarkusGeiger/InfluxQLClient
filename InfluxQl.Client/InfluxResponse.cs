using System.Text.Json.Serialization;

namespace InfluxQl.Client;

// {
//   "results": [
//     {
//       "statement_id": 0,
//       "series": [
//         {
//           "name": "databases",
//           "columns": [
//             "name"
//           ],
//           "values": [
//             [
//               "_internal"
//             ]
//           ]
//         }
//       ]
//     }
//   ]
// }
public class InfluxResponse
{
  [JsonPropertyName("results")]
  public List<InfluxResult> InfluxResults { get; set; } = new List<InfluxResult>();
}

public class InfluxResult
{
  [JsonPropertyName("statement_id")]
  public int StatementId { get; set; }

  [JsonPropertyName("series")]
  public List<InfluxSeries> Series { get; set; } = new List<InfluxSeries>();
}

public class InfluxSeries
{
  [JsonPropertyName("name")]
  public string Name { get; set; } = string.Empty;

  [JsonPropertyName("columns")]
  public List<string> Columns { get; set; } = new List<string>();

  [JsonPropertyName("values")]
  public List<List<string>> Values { get; set; } = new List<List<string>>();
}