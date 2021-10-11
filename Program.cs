using System;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace Youtube_Remote_Controller
{
    class Program
    {
        static public string GetliveChatID(string videoId, YouTubeService youtubeService)
        {
            //引数で取得したい情報を指定
            var videosList = youtubeService.Videos.List("LiveStreamingDetails");
            videosList.Id = videoId;
            //動画情報の取得
            var videoListResponse = videosList.Execute();
            //LiveChatIDを返す
            foreach (var videoID in videoListResponse.Items)
            {
                return videoID.LiveStreamingDetails.ActiveLiveChatId;
            }
            //動画情報取得できない場合はnullを返す
            return null;
        }

        static public async Task GetLiveChatMessage(string liveChatId, YouTubeService youtubeService, string nextPageToken)
        {
            var liveChatRequest = youtubeService.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
            liveChatRequest.PageToken = nextPageToken;

            var liveChatResponse = await liveChatRequest.ExecuteAsync();
            foreach (var liveChat in liveChatResponse.Items)
            {
                try
                {
                    if (liveChat.AuthorDetails.IsChatOwner == true)
                    {
                        Console.WriteLine($"DisplayMessage:{liveChat.Snippet.DisplayMessage},DisplayName:{liveChat.AuthorDetails.DisplayName}");
                        SendKeys.Send("test");

                    }
                }
                catch { }

            }
            await Task.Delay((int)liveChatResponse.PollingIntervalMillis);


            await GetLiveChatMessage(liveChatId, youtubeService, liveChatResponse.NextPageToken);
        }

        static async Task Main(string[] args)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCFXPmw3u_NPaMCZcRsXS2p4dDg6gBtjok"
            });

            Console.Write("遠隔操作を開始するためにはYoutubeの動画のIDを入力してください。：");
            string liveid = Console.ReadLine(); 

            string liveChatId = GetliveChatID(liveid, youtubeService);

            await GetLiveChatMessage(liveChatId, youtubeService, null);

        }
    }
}
