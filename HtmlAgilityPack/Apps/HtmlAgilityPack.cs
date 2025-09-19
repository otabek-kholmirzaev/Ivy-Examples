namespace HtmlAgilityPack.Apps;

using HtmlAgilityPack;

[App(icon: Icons.PartyPopper, title: "HtmlAgilityPack")]
public class HtmlAgilityPackApp : ViewBase
{
    public override object? Build()
    {
        var urlState = UseState("https://ivy.app");
        var urlMetaState = UseState<string>("");
        var urlLinksState = UseState<string>("");
        var urlTitleState = UseState<string>("");
        var parsingState = UseState(false);

        var getTitleData = (string url) =>
        {
            string title = string.Empty;
            var webGet = new HtmlWeb();
            try
            {
                webGet.Load(url);
            }
            catch //invalid url
            {
                return string.Empty;
            }
            var document = webGet.Load(url);
            var titleNode = document.DocumentNode.SelectSingleNode("//head/title");
            if (titleNode != null)
            {
                title = titleNode.InnerText.Trim();
            }
            return title;
        };

        var getMetaData = (string url) =>
        {
            string description = string.Empty;
            var webGet = new HtmlWeb();
            try
            {
                webGet.Load(url);
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
            var document = webGet.Load(url);
            var metaTags = document.DocumentNode.SelectNodes("//meta");
            if (metaTags != null)
            {
                foreach (var tag in metaTags)
                {
                    if (tag.Attributes["name"] != null && tag.Attributes["content"] != null && tag.Attributes["name"].Value.ToLower() == "description")
                    {
                        description += tag.Attributes["content"].Value;
                    }
                }
            }
            else
            {
                description = string.Empty;
            }
            return description;
        };

        var getLinksData = (string url) =>
        {
            string description = string.Empty;
            var webGet = new HtmlWeb();
            try
            {
                webGet.Load(url);
            }
            catch //invalid url
            {
                return string.Empty;
            }
            var document = webGet.Load(url);
            var metaTags = document.DocumentNode.SelectNodes("//a");
            if (metaTags != null)
            {
                foreach (var tag in metaTags)
                {
                    if (tag.Attributes["href"] != null && (tag.Attributes["href"].Value.StartsWith("https://") || tag.Attributes["href"].Value.StartsWith("http://")))
                        description += tag.Attributes["href"].Value + System.Environment.NewLine + System.Environment.NewLine;
                }
            }
            else
            {
                description = string.Empty;
            }
            return description;
        };


        var eventHandler = (Event<Button> e) =>
        {
            urlTitleState.Set("");
            urlMetaState.Set("");
            urlLinksState.Set("");
            parsingState.Set(true);
            urlTitleState.Set(getTitleData(urlState.Value));
            urlMetaState.Set(getMetaData(urlState.Value));
            urlLinksState.Set(getLinksData(urlState.Value));
            parsingState.Set(false);
        };

        return Layout.Vertical().Gap(2).Padding(2)
                   | Text.Block("Enter Site URL:")
                   | urlState.ToTextInput()
                   | new Button("Parse Site HTML", eventHandler).Loading(parsingState.Value)
                   | (urlTitleState.Value.Length > 0 ? Text.Block("Site Title:") : null)
                   | (urlTitleState.Value.Length > 0 ? Text.Code(urlTitleState.Value) : null)     
                   | (urlMetaState.Value.Length > 0 ? Text.Block("Site Meta Data:") : null)
                   | (urlMetaState.Value.Length > 0 ? Text.Code(urlMetaState.Value) : null)
                   | (urlLinksState.Value.Length > 0 ? Text.Block("External Links:") : null)
                   | (urlLinksState.Value.Length > 0 ? Text.Code(urlLinksState.Value) : null)
                 ;
    }
}