using System;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Asystent.common;

namespace Asystent {
	
	public class YouTube {
		private YouTubeService _service;
		private YouTube() { }
		public void Init(string applicationName, string apiKey) {
			_service = new YouTubeService(new BaseClientService.Initializer() {
				ApplicationName = applicationName,
				ApiKey = apiKey
			});
		}
		public VideosEntry SearchVideos(string query) {
			SearchResource.ListRequest listRequest = _service.Search.List("snippet");
			listRequest.Q = query;
			listRequest.MaxResults = 5;
			listRequest.Type = "video";

			SearchListResponse resp = listRequest.Execute();
			
			List<VideoInfo> pool = new List<VideoInfo>();
			foreach (var result in resp.Items) {
				pool.Add(new VideoInfo{
					id = result.Id.VideoId, 
					title = result.Snippet.Title, 
					thumbnail= result.Snippet.Thumbnails.Default__.Url
				});
			}
			VideosEntry videos = new VideosEntry{
				selected = pool[0],
				pool = pool
			};
			return videos;
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