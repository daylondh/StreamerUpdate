namespace StreamerUpdate.API
{
    public class Youtube : StreamProvider
    {
        private YTPoster poster;

        public Youtube()
        {
            this.poster = new YTPoster();

        }

        public void listBroadcasts()
        {
        }
    }
}