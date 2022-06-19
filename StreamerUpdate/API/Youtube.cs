namespace StreamerUpdate.API
{
  public class Youtube : StreamProvider
  {
    private readonly YTPoster _poster;

    public Youtube(YTPoster poster)
    {
      _poster = poster;
    }
    public void listBroadcasts()
    {
    }
  }
}