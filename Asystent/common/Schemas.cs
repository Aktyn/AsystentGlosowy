using System.Collections.Generic;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Asystent.common {
	public struct SpeechResult {
		public string result { get; set; }
		public float confidence { get; set; }
		public ushort type { get; set; }
	}

	public struct MessageSchema {
		public ushort type { get; set; }
		public List<SpeechResult> results { get; set; }
		public ulong result_index { get; set; }
	}

	public struct SpeechResponse {
		public string res { get; set; }
		public ulong index { get; set; }
	}
}