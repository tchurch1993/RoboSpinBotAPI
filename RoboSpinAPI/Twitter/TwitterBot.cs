using System.Net;
using TweetSharp;

namespace RoboSpinAPI.Twitter
{
    public class TwitterBot
    {
        private readonly TwitterService _twitterService;


        public TwitterBot(TwitterBotOptions options)
        {
            this._twitterService = new TwitterService(options.ConsumerKey, options.ConsumerSecret, options.AccessToken, options.AccessSecret);
        }

        public void SendTweet(string message)
        {
            SendTweetOptions options = new SendTweetOptions
            {
                Status = message
            };
            
            IAsyncResult response = _twitterService.SendTweet(options, (/* tweet */_, response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("tweet sent!");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("error sending tweet: " + response.Error.Message);
                    Console.ResetColor();
                }
            });
            response.AsyncWaitHandle.WaitOne();
        }
    }
}
