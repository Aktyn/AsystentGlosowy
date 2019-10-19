using System.Diagnostics;
using System;
using System.IO;

namespace Asystent {
    public class ChromeOpener {
        //private static string os_cmd = isLinux() ? "sh" : "cmd.exe";
        private static string chrome_command = isLinux() ? "google-chrome" : "start chrome";

        private static bool isLinux() {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }

        public static void OpenInStandaloneWindow() {
            try {
                string indexHTML = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
                    "..", "frontend", "build", "index.html"));
                if( !File.Exists(indexHTML) )
                    throw new Exception("index.html not found");
                string arguments = $"--app=\"file://{indexHTML}?closeWithServer=true\" --chrome-frame";
                Console.WriteLine("Executing command: " + chrome_command + " " + arguments);

                //Create process
                Process pProcess = new Process();

                pProcess.StartInfo.FileName = chrome_command;
                pProcess.StartInfo.Arguments = arguments;
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;   
                pProcess.StartInfo.CreateNoWindow = true;
                pProcess.StartInfo.WorkingDirectory = 
                    Path.Combine(Directory.GetCurrentDirectory());
                pProcess.Start();

                string strOutput = pProcess.StandardOutput.ReadToEnd();
                pProcess.WaitForExit();
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}