using System;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Asystent {
	public class VideoInfo {
		public string id { get; set; }
		public string title { get; set; }
	}
	public class YouTube {
		private YouTubeService _service;
		private YouTube() { }
		
		public void Init(string applicationName, string apiKey) {
			_service = new YouTubeService(new BaseClientService.Initializer() {
				ApplicationName = applicationName,
				ApiKey = apiKey
			});
		}
		
		public VideoInfo SearchVideo(string query) {
			SearchResource.ListRequest listRequest = _service.Search.List("snippet");
			listRequest.Q = query;
			listRequest.MaxResults = 1;
			listRequest.Type = "video";
			SearchListResponse resp = listRequest.Execute();
			foreach (var result in resp.Items) {
				//Console.WriteLine("{0}, {1}", result.Snippet.Title, result.Id.VideoId);
				return new VideoInfo{id = result.Id.VideoId, title = result.Snippet.Title};
			}

			return new VideoInfo();
		}
		
		////////////////////////////////////////////

		private static YouTube _instance = null;
		public static YouTube Instance() {
			if (_instance == null)
				_instance = new YouTube();
			return _instance;
		}
	}
}