using MyFilmsPlugin.MyFilms.Configuration;

namespace MyFilmsConfig
{
  public class MyFilmsLauncher : PluginConfigLauncher
  {
    public override string FriendlyPluginName
    {
      get { return "MyFilms"; }
    }

    public override void Launch()
    {
      ConfigConnector plugin = new ConfigConnector();
      plugin.ShowPlugin();
    }
  }
}


