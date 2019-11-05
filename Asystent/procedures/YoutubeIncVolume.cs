﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures {
	public struct IncVolumeSchema {
		public string res { get; set; }
		public string volume { get; set; }
	}
	public class YoutubeIncVolume : ProcedureBase {
		public static Regex regex = new Regex(
			@"(podg[lł]o[sś](nij)?|zwi[eę]ksz) ?(g[lł]o[sś]no[sś][cć]|d[zź]wi[eę]k)?",
			RegexOptions.IgnoreCase | RegexOptions.Compiled);

			public YoutubeIncVolume() { }

			public sealed override void Update(List<SpeechResult> results) {
			Results = results;

			foreach (var res in results) {//ignore interim results
				if ((ResultType)res.type == ResultType.INTERIM)
					return;
			}
			results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);
			
			foreach (var res in results) {
				if (regex.IsMatch(res.result)) {
					string [] split = res.result.Split(new Char [] {' '});
					string vol=null;
					
					for(int i=0; i<split.Length; i++)
					{
						if(Regex.IsMatch(split[i], @"^\d+$"))
						{
							vol=split[i];
							break;
						}
					}
					SendData( new IncVolumeSchema {
							res = "volume_up", 
							volume = vol
						} );
					Finished = true;
					return;
				}
			}
		}
	}
}