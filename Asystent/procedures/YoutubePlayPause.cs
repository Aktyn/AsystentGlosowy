using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Asystent.common;

namespace Asystent.procedures
{
    public class YoutubePlayPause : ProcedureBase
    {
        public static Regex regex = new Regex(
            @"(rozpocznij|wystartuj|wznów|zatrzymaj|zastopuj|stop|pauza|pauzuj) ?(film|utw[oó]r|piosenkę)?",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static Regex regexRes = new Regex(
            @"(rozpocznij|wystartuj|wznów)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public YoutubePlayPause() { }

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
                    SendData(new SimpleResponse
                    {
                        res = regexRes.IsMatch(res.result)?"play":"pause", 
                    });
                    Finished = true;
                    return;
                }
            }
        }
    }
}
