using System.Collections.Generic;

namespace Asystent.common {
	/** MESSAGES FROM CLIENT **/

	public struct SpeechResult {
		public string result { get; set; }
		public float confidence { get; set; }
		public ushort type { get; set; }
	}

	public class MessageSchema {
		public ushort type { get; set; }
	}

	public class SpeechMessageSchema : MessageSchema {
		public List<SpeechResult> results { get; set; }
		public ulong result_index { get; set; }
	}

	public class VideoFinishedMessageSchema : MessageSchema {
		public string video_id { get; set; }
	}

	/** RESPONSES TO CLIENT **/
	
	public struct SpeechResponse {
		public string res { get; set; }
		public ulong index { get; set; }
		public string procedure { get; set; }
	}

	public struct SongRequestSchema {
		public string res { get; set; }
		public VideosEntry videos { get; set; }
	}

	/** COMMON **/

	public class VideosEntry {
		public VideoInfo selected { get; set; }
		public List<VideoInfo> pool { get; set; }
	}

	public struct VideoInfo {
		public string id { get; set; }
		public string title { get; set; }
		public string thumbnail { get; set; }
	}
}