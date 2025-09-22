namespace YamlDotNet.Services;

public class CSharpParser
{
    public Dictionary<string, object> ParseUserInput(string input)
    {
        var data = new Dictionary<string, object>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            
            if (!line.Contains("=")) continue;

            var parts = line.Split('=', 2);
            if (parts.Length != 2) continue;

            var key = parts[0].Trim().TrimEnd(',');
            var value = parts[1].Trim().TrimEnd(',');

            // Simple values
            if (value.StartsWith("\"") && value.EndsWith("\""))
            {
                data[key] = value.Trim('"');
            }
            else if (value == "true" || value == "false")
            {
                data[key] = bool.Parse(value);
            }
            else if (int.TryParse(value, out int intVal))
            {
                data[key] = intVal;
            }
            // Arrays
            else if (line.Contains("new[]"))
            {
                data[key] = ParseArray(value);
            }
            // Dictionaries
            else if (line.Contains("new Dictionary"))
            {
                data[key] = ParseDictionary(line);
            }
            else
            {
                data[key] = value;
            }
        }
        
        return data;
    }

    private string[] ParseArray(string arrayValue)
    {
        var start = arrayValue.IndexOf('{');
        var end = arrayValue.LastIndexOf('}');
        
        if (start < 0 || end <= start) return Array.Empty<string>();
        
        var content = arrayValue.Substring(start + 1, end - start - 1);
        return content.Split(',')
            .Select(item => item.Trim().Trim('"'))
            .Where(item => !string.IsNullOrEmpty(item))
            .ToArray();
    }

    private Dictionary<string, object> ParseDictionary(string dictLine)
    {
        var result = new Dictionary<string, object>();
        
        var start = dictLine.IndexOf('{');
        var end = dictLine.LastIndexOf('}');
        
        if (start < 0 || end <= start) return result;
        
        var content = dictLine.Substring(start + 1, end - start - 1).Trim();
        if (string.IsNullOrWhiteSpace(content)) return result;

        // Simple regex for key-value pairs
        var matches = System.Text.RegularExpressions.Regex.Matches(
            content, 
            @"\{\s*""([^""]+)""\s*,\s*([^,}]+)\s*\}"
        );

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value.Trim();

            if (int.TryParse(value, out int intVal))
                result[key] = intVal;
            else if (bool.TryParse(value, out bool boolVal))
                result[key] = boolVal;
            else if (value.StartsWith("\"") && value.EndsWith("\""))
                result[key] = value.Trim('"');
            else
                result[key] = value;
        }

        return result;
    }
}
