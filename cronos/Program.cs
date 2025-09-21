CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
var server = new Server(new ServerArgs
{
    DefaultAppId = "CronosApp"
});
#if DEBUG
server.UseHotReload();
#endif
server.AddAppsFromAssembly();
server.AddConnectionsFromAssembly();
var chromeSettings = new ChromeSettings().UseTabs(preventDuplicates: true);
server.UseChrome(chromeSettings);
await server.RunAsync();