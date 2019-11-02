using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public struct SongRequestSchema {
		public string res { get; set; }
		public string video_id { get; set; }
		public string title { get; set; }
	}
	public class YoutubePlay : ProcedureBase {
		public static Regex regex = new Regex(
			@"(zagraj|graj|odtw[oó]rz|odtwarzaj) ?(piosenk[eę]|utw[oó]r)? (.+)",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);
		
		public YoutubePlay() { }
		
		public sealed override void Update(List<SpeechResult> results) {
			Results = results;

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			
			//sort in place by confidence (DESC)
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				if (regex.IsMatch(res.result)) {
					var match = regex.Match(res.result);
					if (match.Success && match.Groups.Count > 0) {
						var user_query = match.Groups[match.Groups.Count-1].Value;
						
						VideoInfo video = YouTube.Instance().SearchVideo( user_query );
						SendData( new SongRequestSchema {
							res = "request_song", 
							video_id = video.id, 
							title = video.title
						} );
						Finished = true;
						return;
					}
				}
			}
		}
	}
}