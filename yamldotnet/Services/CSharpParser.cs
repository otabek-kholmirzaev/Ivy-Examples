namespace YamlDotNet.Services;

public class CSharpParser
{
    public Dictionary<string, object> ParseUserInput(string input)
    {
        var data = new Dictionary<string, object>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < lines.Length; i++)
        {
            var trimmedLine = lines[i].Trim();

            // Simple assignment
            if (trimmedLine.Contains("=") &&
                !trimmedLine.Contains("var") &&
                !trimmedLine.Contains("new Dictionary") &&
                !trimmedLine.Contains("new[]"))
            {
                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().TrimEnd(',');
                    var value = parts[1].Trim().TrimEnd(',');

                    if (value.StartsWith("\"") && value.EndsWith("\""))
                        data[key] = value.Trim('"');
                    else if (value == "true" || value == "false")
                        data[key] = bool.Parse(value);
                    else if (int.TryParse(value, out int intValue))
                        data[key] = intValue;
                    else if (float.TryParse(value, out float floatValue))
                        data[key] = floatValue;
                    else
                        data[key] = value;
                }
            }

            // Array
            else if (trimmedLine.Contains("new[]"))
            {
                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().TrimEnd(',');
                    var arrayValue = parts[1].Trim().TrimEnd(',');

                    var startIndex = arrayValue.IndexOf('{');
                    var endIndex = arrayValue.LastIndexOf('}');
                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        var arrayContent = arrayValue.Substring(startIndex + 1, endIndex - startIndex - 1);
                        var items = arrayContent.Split(',')
                            .Select(item => item.Trim().Trim('"'))
                            .Where(item => !string.IsNullOrEmpty(item))
                            .ToArray();
                        data[key] = items;
                    }
                }
            }
            
            // Dictionary
            else if (trimmedLine.Contains("new Dictionary"))
            {
                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().TrimEnd(',');
                    
                    var dictLines = new List<string> { lines[i] };
                    
                    // Handle single-line dictionary (like empty dictionary)
                    if (trimmedLine.Contains("}"))
                    {
                        var dict = ParseDictionary(trimmedLine);
                        data[key] = dict;
                    }
                    else
                    {
                        // Multi-line dictionary
                        for (int j = i + 1; j < lines.Length; j++)
                        {
                            dictLines.Add(lines[j]);
                            if (lines[j].Contains("}"))
                            {
                                i = j; // Skip to end of dictionary
                                break;
                            }
                        }
                        
                        var fullDictString = string.Join(" ", dictLines);
                        var dict = ParseDictionary(fullDictString);
                        data[key] = dict;
                    }
                }
            }
        }
        return data;
    }

    private Dictionary<string, object> ParseDictionary(string dictString)
    {
        var result = new Dictionary<string, object>();

        var startIndex = dictString.IndexOf('{');
        var endIndex = dictString.LastIndexOf('}');
        if (startIndex < 0 || endIndex <= startIndex) return result;

        var content = dictString.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();

        // Handle empty dictionary
        if (string.IsNullOrWhiteSpace(content))
        {
            return result; // Return empty dictionary
        }

        // Try multiple parsing approaches
        var matches = System.Text.RegularExpressions.Regex.Matches(
            content,
            @"\{\s*""([^""]+)""\s*,\s*([^,}]+?)\s*\}(?:\s*,)?"
        );

        if (matches.Count > 0)
        {
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var key = match.Groups[1].Value;
                var valueRaw = match.Groups[2].Value.Trim();

                if (int.TryParse(valueRaw, out var intVal))
                    result[key] = intVal;
                else if (bool.TryParse(valueRaw, out var boolVal))
                    result[key] = boolVal;
                else if (valueRaw.StartsWith("\"") && valueRaw.EndsWith("\""))
                    result[key] = valueRaw.Trim('"');
                else
                    result[key] = valueRaw;
            }
        }
        else
        {
            // Fallback: try to parse manually by looking for key-value pairs
            
            // Split by }, but be careful about nested braces
            var pairs = SplitDictionaryPairs(content);
            foreach (var pair in pairs)
            {
                var parts = pair.Split(',');
                if (parts.Length >= 2)
                {
                    var key = parts[0].Trim().Trim('{').Trim().Trim('"');
                    var value = parts[1].Trim().Trim('}').Trim();
                    
                    if (int.TryParse(value, out var intVal))
                        result[key] = intVal;
                    else if (bool.TryParse(value, out var boolVal))
                        result[key] = boolVal;
                    else if (value.StartsWith("\"") && value.EndsWith("\""))
                        result[key] = value.Trim('"');
                    else
                        result[key] = value;
                }
            }
        }

        return result;
    }

    private List<string> SplitDictionaryPairs(string content)
    {
        var pairs = new List<string>();
        var currentPair = "";
        var braceCount = 0;
        
        for (int i = 0; i < content.Length; i++)
        {
            var c = content[i];
            
            if (c == '{')
            {
                braceCount++;
                currentPair += c;
            }
            else if (c == '}')
            {
                braceCount--;
                currentPair += c;
                
                if (braceCount == 0)
                {
                    pairs.Add(currentPair.Trim());
                    currentPair = "";
                }
            }
            else
            {
                currentPair += c;
            }
        }
        
        return pairs;
    }
}
