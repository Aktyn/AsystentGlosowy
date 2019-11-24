using System;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace Asystent {
	public class VideoInfo {
		public string id { get; set; }
		public string title { get; set; }
		public string thumbnail { get; set; }
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
		public List<VideoInfo> videof = new List<VideoInfo>();
		public List<VideoInfo> SearchVideos(string query) {
			SearchResource.ListRequest listRequest = _service.Search.List("snippet");
			listRequest.Q = query;
			listRequest.MaxResults = 5;
			listRequest.Type = "video";
			SearchListResponse resp = listRequest.Execute();
			foreach (var result in resp.Items) {
				videof.Add(new VideoInfo{id = result.Id.VideoId, title = result.Snippet.Title, thumbnail= result.Snippet.Thumbnails.Default__.Url});
			}
			return videof;
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