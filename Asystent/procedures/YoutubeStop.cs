using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures
{
    public struct YoutubeStopSchema
    {
        public string res { get; set; }
    }
    public class YoutubeStop : ProcedureBase
    {
        public static Regex regex = new Regex(
            @"(zatrzymaj|zastopuj|stop) ?(film|utwor|piosenkę)?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public YoutubeStop() { }

        public sealed override void Update(List<SpeechResult> results)
        {
            Results = results;

            foreach (var res in results)
            {//ignore interim results
                if ((ResultType)res.type == ResultType.INTERIM)
                    return;
            }
            results.Sort((res1, res2) => res1.confidence > res2.confidence ? 0 : 1);

            foreach (var res in results)
            {
                if (regex.IsMatch(res.result))
                {
                    SendData(new YoutubeStopSchema
                    {
                        res = "pause"
                    });
                    Finished = true;
                    return;
                }
            }
        }
    }
}
