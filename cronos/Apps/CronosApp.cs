using Cronos;

namespace CronosExample.Apps;

[App(icon: Icons.TimerReset, title: "Cronos")]
public class CronosApp : ViewBase
{
    public override object? Build()
    {
        var client = UseService<IClientProvider>();

        var inputCroneExpression = UseState((string?)null);

        var timeZones = TimeZoneInfo.GetSystemTimeZones()
            .Select(tz => new { Id = tz.Id, Name = tz.DisplayName })
            .ToList();
        var inputTimeZone = UseState(timeZones[0].Id);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(inputTimeZone.Value);


        var nextOccurrence = UseState<DateTime?>();


        void TryParseCron()
        {
            if (!string.IsNullOrWhiteSpace(inputCroneExpression.Value))
            {
                try
                {
                    var croneExpression = CronExpression.Parse(inputCroneExpression.Value);
                    var next = croneExpression.GetNextOccurrence(DateTimeOffset.Now, timeZone);
                    nextOccurrence.Value = next?.DateTime;
                }
                catch (CronFormatException ex)
                {
                    client.Toast($"Invalid CRON: {ex.Message}");
                }
            }
        }

        return Layout.Vertical(
                Text.H1("Cronos"),
                
                Text.Label("Select a time zone"),
                inputTimeZone
                    .ToSelectInput(timeZones
                        .Select(tz => new Option<string>(tz.Name, tz.Id))
                        .ToList()),
                
                Text.Label("Enter a cron expression"),
                Layout.Horizontal(
                    inputCroneExpression
                        .ToTextInput()
                        .Placeholder("Cron expression (e.g. \"*/5 * * * *\")"),
                
                    new Button("Try parse", onClick: TryParseCron)
                        .Disabled(string.IsNullOrWhiteSpace(inputCroneExpression.Value))
                    ),
                
                Text.H3($"Next occurrence: {nextOccurrence.Value?.ToString("dd.MM.yyyy HH:mm:ss zzz") ?? "—"}")
            ).Width(Size.Units(170))
            .Padding(20, 0, 20, 20);
    }
}