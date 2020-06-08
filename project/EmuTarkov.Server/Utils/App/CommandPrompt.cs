using System.Diagnostics;
using System.Text;
using EmuTarkov.Common.Utils.App;

namespace EmuTarkov.Server.Utils.App
{
    public class CommandPrompt
    {
        /// <summary>
        /// Execute command in Command Prompt (cmd.exe)
        /// </summary>
        public static void ExecuteCommand(string action)
        {
            StringBuilder stringBuilder = new StringBuilder();

            using (Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = "cmd.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = "/c " + action
                }
            })
            {
                Logger.LogInfo($"EmuTarkov.Server: Executing Command: {action}");
                process.Start();

                while (!process.StandardOutput.EndOfStream)
                {
                    stringBuilder.AppendLine(process.StandardOutput.ReadLine());
                }

                process.Close();
            }

            // for debugging
            Logger.LogInfo(stringBuilder.ToString());
        }
    }
}
