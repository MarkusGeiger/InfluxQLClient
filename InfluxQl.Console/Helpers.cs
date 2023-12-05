using System.Data;
using Spectre.Console;

public class Helpers
{
  public static void PrintTable(string title, List<string> columns, List<List<object>> dataTable)
  {
    var table = new Table();
    table.AddColumns(columns.ToArray());
    foreach(var row in dataTable)
    {
      table.AddRow(row.Select(val => val.ToString() ?? string.Empty).ToArray());
    }

    table.Title = new TableTitle(title);

    AnsiConsole.Write(table);
  }
}